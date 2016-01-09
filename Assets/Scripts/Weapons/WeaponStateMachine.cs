using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public enum WeaponState
    {
        Inactive,
        Firing,
        Cooldown
    }

    public class WeaponStateMachine
    {
        private readonly float _weaponCooldown;
        private readonly float _weaponHitTime;

        private float _lastFireTimeStamp;
        private float _timeUntilHitStops;

        public WeaponStateMachine(float weaponCooldown, float weaponHitTime)
        {
            _weaponCooldown = weaponCooldown;
            _weaponHitTime = weaponHitTime;
        }

        public bool TryFire()
        {
            var state = GetState();
            if (state == WeaponState.Inactive)
            {
                _lastFireTimeStamp = Time.time;
                _timeUntilHitStops = Time.time + _weaponHitTime;
                return true;
            }

            return false;
        }

        public WeaponState GetState()
        {
            if (Time.time < _timeUntilHitStops) return WeaponState.Firing;
            if (Time.time < _lastFireTimeStamp + _weaponCooldown) return WeaponState.Cooldown;

            return WeaponState.Inactive;
        }
    }
}
