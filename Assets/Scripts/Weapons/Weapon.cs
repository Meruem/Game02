using System;
using System.Collections.Generic;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(PolygonCollider2D))]
    class Weapon : MonoBehaviour
    {
        public int WeaponDamage = 2;
        public float WeaponCooldown = 1; // In seconds
        public float WeaponHitTime = 0.5f;

        private PolygonCollider2D _collider;
        private readonly List<int> _alreadyHitTargets = new List<int>();

        private WeaponStateMachine _weaponStateMachine;

        void Start()
        {
            _collider = GetComponent<PolygonCollider2D>();
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
                }
            }

            _collider.enabled = _weaponStateMachine.GetState() == WeaponState.Firing;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // prevent multihits of same target
            // Debug.LogFormat("Collided with {0}", other.GetInstanceID());
            if (_alreadyHitTargets.Contains(other.GetInstanceID())) return;
            _alreadyHitTargets.Add(other.GetInstanceID());

            var takeDamage = other.GetComponent<TakeDamageTrigger>();
            if (takeDamage != null)
            {
                takeDamage.TakeDamage(WeaponDamage);
            }
        }
    }
}
