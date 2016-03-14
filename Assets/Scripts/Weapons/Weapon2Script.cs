using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Assets.Scripts.Weapons;
using UnityEngine;

[Serializable]
public class ForcedMove
{
    public float Time;
    public float Speed;
    public bool AllowOtherMovement = false; //Only provides delay in state machine execution
}

[RequireComponent(typeof(Animator))]
public class Weapon2Script : MonoBehaviour, IAttack
{
    public int WeaponDamage = 2;
    public int AfterBlockEnergyDamage = 10;
    public int EnergyRequired = 20;
    public int StabilityDamage = 10;

    public List<ForcedMove> ReadyMoves;
    public List<ForcedMove> SwingMoves;
    public List<ForcedMove> RecoverMoves;

    public int RotationAdjustment = -90;

    public List<int> AlreadyHitTargets { get; private set; }

    public bool CanFire { get; private set; }

    private bool _isBlocked;
    private Animator _animator;
    private int _id;
    private bool _isCanceled;

    public void Start()
    {
        CanFire = true;
        _id = gameObject.GetInstanceID();
        _animator = GetComponent<Animator>();
        AlreadyHitTargets = new List<int>();

        this.GetPubSub().SubscribeInContext<WeaponBlockedMessage>(m => ShieldHit());
    }

    public int Id
    {
        get { return _id; }
    }

    public int RequiredEnergy
    {
        get { return EnergyRequired; }
    }

    public void Fire()
    {
        // Hit started
        if (CanFire)
        {
            StartCoroutine(Swing());
        }
    }

    public void CancelAttack()
    {
        if (CanFire) return; // not attacking
        _isCanceled = true;

        _animator.SetBool("IsCanceled", true);
    }

    private void ShieldHit()
    {
        _isBlocked = true;
        _animator.SetBool("IsSwinging", false);
        _animator.SetBool("IsBlocked", true);
        this.GetPubSub().PublishMessageInContext(new ForceMovementMessage(Vector2.zero, 0, 0, false));
    }

    private IEnumerator Swing()
    {
        CanFire = false;
        _isBlocked = false;
        _isCanceled = false;
        _animator.SetBool("IsCanceled", false);
        _animator.SetBool("IsBlocked", false);
        AlreadyHitTargets.Clear();
        var direction = Math2.AngleDegToVector(transform.rotation.eulerAngles.z + RotationAdjustment);

        // ready phase
        if (!_isCanceled)
        {
            _animator.SetBool("IsRedying", true);
            if (ReadyMoves != null)
            {
                for (var i = 0; i < ReadyMoves.Count; i++)
                {
                    if (_isCanceled) break;
                    var move = ReadyMoves[i];
                    this.GetPubSub()
                        .PublishMessageInContext(new ForceMovementMessage(direction, move.Speed, move.Time,
                            move.AllowOtherMovement));
                    yield return new WaitForSeconds(move.Time);
                }
            }
        }
        _animator.SetBool("IsRedying", false);

        // swing phase
        if (!_isCanceled)
        {
            _animator.SetBool("IsSwinging", true);
            if (SwingMoves != null)
            {
                direction = Math2.AngleDegToVector(transform.rotation.eulerAngles.z + RotationAdjustment); //recalculate
                for (var i = 0; i < SwingMoves.Count; i++)
                {
                    if (_isBlocked || _isCanceled) break;
                    var move = SwingMoves[i];
                    this.GetPubSub()
                        .PublishMessageInContext(new ForceMovementMessage(direction, move.Speed, move.Time,
                            move.AllowOtherMovement));
                    yield return new WaitForSeconds(move.Time);
                }
            }
        }
        _animator.SetBool("IsSwinging", false);

        //recover phase
        if (!_isCanceled)
        {
            direction = Math2.AngleDegToVector(transform.rotation.eulerAngles.z + RotationAdjustment); // recaulculate
            _animator.SetBool("IsRecovering", true);
            if (RecoverMoves != null)
            {
                for (var i = 0; i < RecoverMoves.Count; i++)
                {
                    if (_isCanceled) break;
                    var move = RecoverMoves[i];
                    this.GetPubSub()
                        .PublishMessageInContext(new ForceMovementMessage(direction, move.Speed, move.Time,
                            move.AllowOtherMovement));
                    yield return new WaitForSeconds(move.Time);
                }
            }
        }
        _animator.SetBool("IsRecovering", false);
        CanFire = true;

        this.GetPubSub().PublishMessageInContext(new AttackEndedMessage());
    }
}

