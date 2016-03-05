using System;
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

[RequireComponent(typeof(IMoveScript))]
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

    private IMoveScript _moveScript;
    private ToMoveDirectionRotation _toMoveDirectionRotation;
    private ToTargetObjectRotation _toTargetObjectRotation;

    private bool _gunCooldown;

    public void Awake()
    {
        _dynamicGameObjects = GameObjectEx.Find(GameObjectNames.DynamicObjects);
        _playerGameObject = GameObjectEx.FindGameObjectWithTag(GameObjectTags.Player);
        _toMoveDirectionRotation = GetComponent<ToMoveDirectionRotation>();
        _toTargetObjectRotation = GetComponent<ToTargetObjectRotation>();

        var shadowLayer = Layers.GetLayer(LayerName.ShadowLayer);
        _layerMask = (1 << shadowLayer) | (1 << Layers.GetLayer(LayerName.Player));
        _moveScript = GetComponent<IMoveScript>();
    }

    public void Start ()
	{
        this.GetPubSub().SubscribeInContext<WeaponHitMessage>(m => HandleWeaponHit((WeaponHitMessage)m));
	    StartCoroutine(WanderAround());
    }

    private void HandleWeaponHit(WeaponHitMessage weaponHitMessage)
    {
        Stats.AddAmount(StatsEnum.Health, -weaponHitMessage.Damage);
    }

    private void FireBullet(float angle)
    {
        if (BulletPrototype.Prefab == null)
        {
            Debug.LogWarning("Prototype is null.");
        }
        else
        {
            BulletObjectFactory.CreateBullet(transform.position, angle, BulletPrototype, Layers.GetLayer(LayerName.MonsterBullets), _dynamicGameObjects.transform);
        }
    }

    private void Move(Vector2 vector)
    {
        if (_toMoveDirectionRotation != null)
        {
            _toMoveDirectionRotation.MoveBackwards = false;
        }
        _moveScript.MoveMaxSpeed(vector);
    }

    private void MoveBackWards(Vector2 vector)
    {
        if (_toMoveDirectionRotation != null)
        {
            _toMoveDirectionRotation.MoveBackwards = true;
        }
        _moveScript.MoveMaxSpeed(vector);
    }

    private void Move(Vector2 vector, float speed)
    {
        if (_toMoveDirectionRotation != null)
        {
            _toMoveDirectionRotation.MoveBackwards = false;
        }
        _moveScript.MoveNormal(vector, speed);
    }
    
    private void Attack()
    {
    	this.GetPubSub().PublishMessageInContext(new FireMessage(true));
    }

    IEnumerator WeaponCooldown(float time)
    {
        _gunCooldown = true;
        yield return new WaitForSeconds(time);
        _gunCooldown = false;
    }

    IEnumerator WanderAround()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            if (VisibilityHelper.CheckVisibility(transform, _playerGameObject.transform, MaxVisibleDistance, _layerMask))
            {
                if (_toTargetObjectRotation != null)
                {
                    _toTargetObjectRotation.Target = _playerGameObject.transform;
                    _toTargetObjectRotation.enabled = true;
                }
                if (_toMoveDirectionRotation != null)
                {
                    _toMoveDirectionRotation.enabled = false;
                }

                if (Stats.HasEnaugh(StatsEnum.Bullets, 1) && !_gunCooldown)
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
                    var distance = (transform.position - _playerGameObject.transform.position).magnitude;
                    if (distance < TooCloseDistance)
                    {
                        var vector = -_playerGameObject.transform.position + transform.position;
                        MoveBackWards(vector);
                    }
                    else if (distance <= AttackDistance)
                    {
                        Attack();
                    }
                    else
                    {
                        var vector = _playerGameObject.transform.position - transform.position;
                        Move(vector);
                    }
                    yield return null;
                }
            }
            else
            {
                if (_toTargetObjectRotation != null)
                {
                    _toTargetObjectRotation.enabled = false;
                }
                if (_toMoveDirectionRotation != null)
                {
                    _toMoveDirectionRotation.enabled = true;
                }

                float angle = Random.Range(0, 16) * (float)Math.PI * 2f / 16;
                var vector = Math2.AngleRadToVector(angle);
                Move(vector);

                yield return new WaitForSeconds(Random.Range(1, 3));
            }
        }
    }
}
