using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Assets.Scripts.Weapons;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Weapon2Script : MonoBehaviour
{
    public int WeaponDamage = 2;
    public float WeaponCooldown = 1; // In seconds
    public float WeaponHitTime = 0.5f;

    //private PolygonCollider2D _collider;
    private Animator _animator;
    private readonly List<int> _alreadyHitTargets = new List<int>();

    private WeaponStateMachine _weaponStateMachine;

    void Start()
    {
        _animator = GetComponent<Animator>();
        //_collider = GetComponent<PolygonCollider2D>();
        _weaponStateMachine = new WeaponStateMachine(WeaponCooldown, WeaponHitTime);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Hit started
            if (_weaponStateMachine.TryFire())
            {
                _alreadyHitTargets.Clear();
                _animator.SetBool("IsSwinging", true);
            }
        }

        _animator.SetBool("IsSwinging", _weaponStateMachine.GetState() == WeaponState.Firing);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // prevent multihits of same target
        // Debug.LogFormat("Collided with {0}", other.GetInstanceID());
        if (_alreadyHitTargets.Contains(other.GetInstanceID())) return;
        _alreadyHitTargets.Add(other.GetInstanceID());

        other.GetPubSub().PublishMessageInContext(new TakeDamageMessage(WeaponDamage));
    }
}

