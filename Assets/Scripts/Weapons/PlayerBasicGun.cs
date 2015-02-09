using UnityEngine;

namespace Assets.Scripts.Weapons
{
    class PlayerBasicGun : WeaponBase
    {
        private readonly AmmoContainer _ammoContainer;

        private BulletPrototype _bulletPrototype = new BulletPrototype
        {
            Damage = 3,
            Speed = 10,
            PrefabName = "Bullet"
        };

        public PlayerBasicGun(AmmoContainer ammoContainer)
        {
            _ammoContainer = ammoContainer;
        }

        public override string Name
        {
            get { return "Basic Gun"; }
        }

        public override void Fire(Vector2 position, float degAngle)
        {
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
                    Transform.Instantiate(Resources.Load<Transform>(_bulletPrototype.PrefabName), position,
                        Quaternion.Euler(0, 0, degAngle + 90));
            bullet.gameObject.layer = (int) Layers.Player;
            var bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Damage = _bulletPrototype.Damage;
            bulletScript.Speed = _bulletPrototype.Speed;
            bullet.rigidbody2D.velocity = vector*_bulletPrototype.Speed;
        }
    }
}
