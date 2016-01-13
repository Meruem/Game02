using UnityEngine;

namespace Assets.Scripts.Weapons
{
    class PlayerBasicGun : WeaponBase
    {
        private readonly AmmoContainer _ammoContainer;
        private readonly WeaponStateMachine _weaponStateMachine;

        private readonly BulletPrototype _bulletPrototype = new BulletPrototype
        {
            Damage = 3,
            Speed = 10,
            PrefabName = "Bullet"
        };

        public PlayerBasicGun(AmmoContainer ammoContainer, float weaponCooldown)
        {
            _ammoContainer = ammoContainer;
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
                var bullet = BulletObjectFactory.CreateBullet(position, degAngle, _bulletPrototype);
                bullet.gameObject.layer = (int) Layers.PlayerBullets;
                _ammoContainer.RemoveAmmo(AmmoType.Bullets, 1);
            }

            base.Fire(position, degAngle);
        }
    }
}
