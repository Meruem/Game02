﻿using UnityEngine;

namespace Assets.Scripts.Weapons
{
    class PlayerBasicGun : WeaponBase
    {
        private readonly AmmoContainer _ammoContainer;
        private readonly float _weaponCooldown;
        private readonly WeaponStateMachine _weaponStateMachine;

        private BulletPrototype _bulletPrototype = new BulletPrototype
        {
            Damage = 3,
            Speed = 10,
            PrefabName = "Bullet"
        };

        public PlayerBasicGun(AmmoContainer ammoContainer, float weaponCooldown)
        {
            _ammoContainer = ammoContainer;
            _weaponCooldown = weaponCooldown;
            _weaponStateMachine = new WeaponStateMachine(weaponCooldown, 0);
        }

        public override string Name
        {
            get { return "Basic Gun"; }
        }

        public override void Fire(Vector2 position, float degAngle)
        {
            if (!_weaponStateMachine.TryFire()) return;

            Debug.Log("Bullets: " + _ammoContainer.AmmoAmmount(AmmoType.Bullets));
            if (_ammoContainer.HasEnaughAmmo(AmmoType.Bullets, 1))
            {
                CreateBullet(position, degAngle);
                _ammoContainer.RemoveAmmo(AmmoType.Bullets, 1);
            }

            base.Fire(position, degAngle);
        }

        private void CreateBullet(Vector2 position, float degAngle)
        {
            var vector = Quaternion.AngleAxis(degAngle, Vector3.forward)*Vector3.up;

            var bullet =
                (Transform)
                    Object.Instantiate(Resources.Load<Transform>(_bulletPrototype.PrefabName), position,
                        Quaternion.Euler(0, 0, degAngle + 90));
            bullet.gameObject.layer = (int) Layers.Player;
            var bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Damage = _bulletPrototype.Damage;
            bulletScript.Speed = _bulletPrototype.Speed;
            bullet.GetComponent<Rigidbody2D>().velocity = vector*_bulletPrototype.Speed;
        }
    }
}
