﻿using System;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Actors.Movement;
using Assets.Scripts.Actors.Stats;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Assets.Scripts.Visibility;
using Assets.Scripts.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MoveScript))]
public class MonsterController : MonoBehaviour
{
    public Stats Stats;
    public BulletPrototype BulletPrototype;
    public float MaxVisibleDistance = 4;
    public float AttackDistance = 0.6f;
    public float TooCloseDistance = 0.3f;

    private GameObject _dynamicGameObjects;
    private GameObject _playerGameObject;
    private int _layerMask;

    private MoveScript _moveScript;
    private ToMoveDirectionRotation _toMoveDirectionRotation;
    private ToTargetObjectRotation _toTargetObjectRotation;

    private WeaponManager _weaponManager;

    private bool _gunCooldown;
    private bool _isStaggered;
    private bool _isAttacking;

    public void Awake()
    {
        _dynamicGameObjects = GameObjectEx.Find(GameObjectNames.DynamicObjects);
        _playerGameObject = GameObjectEx.FindGameObjectWithTag(GameObjectTags.Player);
        _toMoveDirectionRotation = GetComponent<ToMoveDirectionRotation>();
        _toTargetObjectRotation = GetComponent<ToTargetObjectRotation>();

        var shadowLayer = Layers.GetLayer(LayerName.ShadowLayer);
        _layerMask = (1 << shadowLayer) | (1 << Layers.GetLayer(LayerName.Player));
        _moveScript = GetComponent<MoveScript>();
        _weaponManager = GetComponentInChildren<WeaponManager>();

        if (_weaponManager == null)
        {
            Debug.LogError("Weapon manager not found in child objects.");
        }
    }

    public void Start()
	{
        this.GetPubSub().SubscribeInContext<WeaponHitMessage>(m => HandleWeaponHit((WeaponHitMessage)m));

        // On Stagger
        this.GetPubSub().SubscribeInContext<StaggeredMessage>(m => HandleStaggerMessage((StaggeredMessage)m));

        this.GetPubSub().SubscribeInContext<AttackEndedMessage>(m => HandleAttackEnded());

        Stats.AddRegen(StatsEnum.Stability, 10);

        // Start AI
        StartCoroutine(AICoroutine());
    }

    private void HandleAttackEnded()
    {
        _isAttacking = false;
    }

    private void HandleStaggerMessage(StaggeredMessage message)
    {
        _isStaggered = true;
        // disable movement
        this.GetPubSub().PublishMessageInContext(new ForceMovementMessage(Vector2.zero, 0, message.Time, false));

        _weaponManager.CancelAttack();
        this.StartAfterTime(() => { _isStaggered = false; }, message.Time);
    }

    private void HandleWeaponHit(WeaponHitMessage weaponHitMessage)
    {
        Stats.AddAmount(StatsEnum.Health, -weaponHitMessage.Damage);
        Stats.AddAmount(StatsEnum.Stability, -weaponHitMessage.StabilityDamage);
    }

    private void FireBullet(float angle)
    {
        if (_isStaggered) return;

        if (BulletPrototype.Prefab == null)
        {
            Debug.LogWarning("Prototype is null.");
        }
        else
        {
            Bullet.CreateBullet(transform.position, angle, BulletPrototype, Layers.GetLayer(LayerName.MonsterBullets), _dynamicGameObjects.transform);
        }
    }

    private void Move(Vector2 vector)
    {
        if (_isStaggered) return;

        if (_toMoveDirectionRotation != null)
        {
            _toMoveDirectionRotation.MoveBackwards = false;
        }
        _moveScript.MoveMaxSpeed(vector);
    }

    private void MoveBackWards(Vector2 vector)
    {
        if (_isStaggered) return;

        if (_toMoveDirectionRotation != null)
        {
            _toMoveDirectionRotation.MoveBackwards = true;
        }
        _moveScript.MoveMaxSpeed(vector);
    }
    
    private void Attack()
    {
        if (_isStaggered) return;
        _weaponManager.FirePrimary();
        _isAttacking = true;
    }

    IEnumerator WeaponCooldown(float time)
    {
        _gunCooldown = true;
        yield return new WaitForSeconds(time);
        _gunCooldown = false;
    }

    IEnumerator AICoroutine()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            if (VisibilityHelper.CheckVisibility(transform, _playerGameObject.transform, MaxVisibleDistance, _layerMask))
            {       
                // player visible
                if (_toTargetObjectRotation != null && !_toTargetObjectRotation.enabled)
                {
                    // point to player
                    _toTargetObjectRotation.Target = _playerGameObject.transform;
                    _toTargetObjectRotation.enabled = true;
                }
                if (_toMoveDirectionRotation != null && _toMoveDirectionRotation.enabled)
                {
                    _toMoveDirectionRotation.enabled = false;
                }

                // shoot if able
                if (!_isStaggered && Stats.HasEnough(StatsEnum.Bullets, 1) && !_gunCooldown)
                {
                    var q = Quaternion.LookRotation(Vector3.forward,
                        _playerGameObject.transform.position - transform.position);
                    FireBullet(q.eulerAngles.z);
                    Stats.AddAmount(StatsEnum.Bullets, -1);
                    StartCoroutine(WeaponCooldown(2));

                    yield return null;
                }
                else
                {
                    // go to proper distance
                    var distance = (transform.position - _playerGameObject.transform.position).magnitude;
                    if (distance < TooCloseDistance && !_isAttacking)
                    {
                        // move back when close
                        var vector = -_playerGameObject.transform.position + transform.position;
                        MoveBackWards(vector);
                    }
                    else if (distance <= AttackDistance && !_isAttacking)
                    {
                        // attack when in attack range
                        Attack();
                    }
                    else
                    {
                        // move to target
                        var vector = _playerGameObject.transform.position - transform.position;
                        Move(vector);
                    }
                    yield return null;
                }
            }
            else
            {
                if (_toTargetObjectRotation != null && _toTargetObjectRotation.enabled)
                {
                    _toTargetObjectRotation.enabled = false;
                }
                if (_toMoveDirectionRotation != null && !_toMoveDirectionRotation.enabled)
                {
                    _toMoveDirectionRotation.enabled = true;
                }

                // wander randomly
                float angle = Random.Range(0, 16) * (float)Math.PI * 2f / 16;
                var vector = Math2.AngleRadToVector(angle);
                Move(vector);

                yield return new WaitForSeconds(Random.Range(1, 3));
            }
        }
    }
}
