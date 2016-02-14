using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Weapon2Script : MonoBehaviour
{
    public int WeaponDamage = 2;
    public float WeaponCooldown = 1; // From moment weapon is fired
    public float WeaponHitTime = 0.5f; // Max -> animation can end before
    public float ForcedForwardTime = 0.3f;
    public float ForcedSpeed = 3;
    public float ForcedStopTime = 0.2f;
    public int RotationAdjustment = -90;
    public float ReadyTime = 0.05f;

    public List<int> AlreadyHitTargets { get; private set; }

    //private WeaponStateMachine _weaponStateMachine;
    private bool _canFire = true;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        //_weaponStateMachine = new WeaponStateMachine(WeaponCooldown, WeaponHitTime);
        AlreadyHitTargets = new List<int>();
        this.GetPubSub().SubscribeInContext<FireMessage>(m =>
        {
            if (((FireMessage)m).IsSecondary) Fire();
        });
    }

    private void Fire()
    {
        // Hit started
        if (_canFire)
        {
            StartCoroutine(Swing());
        }
    }

    private IEnumerator Swing()
    {
        _canFire = false;
        AlreadyHitTargets.Clear();
        _animator.SetBool("IsRedying", true);
        yield return new WaitForSeconds(ReadyTime);
        _animator.SetBool("IsRedying", false);
        _animator.SetBool("IsSwinging", true);
        this.GetPubSub()
            .PublishMessageInContext(
                new ForceMovementMessage(
                    Math2.AngleDegToVector(transform.rotation.eulerAngles.z + RotationAdjustment), ForcedSpeed,
                    ForcedForwardTime, ForcedStopTime));

        yield return new WaitForSeconds(WeaponHitTime);
        _animator.SetBool("IsSwinging", false);
        _animator.SetBool("IsRecovering", true);
        yield return new WaitForSeconds(WeaponCooldown);
        _animator.SetBool("IsRecovering", false);
        _canFire = true;
    }
}

