using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class PlayerBasicGun : MonoBehaviour
    {
        public AmmoContainer AmmoContainer; 
        public float WeaponCooldown;    // Time until next shot is ready
        public BulletPrototype BulletPrototype;

        public List<BulletPrototype> List;

        private WeaponStateMachine _weaponStateMachine;
        private GameObject _dynamicGameObjects;

        public void Awake()
        {
            _weaponStateMachine = new WeaponStateMachine(WeaponCooldown, 0);
            _dynamicGameObjects = GameObject.Find("Dynamic Objects");
        }

        public void Fire(Vector2 position, float degAngle)
        {
            if (AmmoContainer == null)
            {
                Debug.LogWarning("Ammo container not found for this gun!");
            }
            else if (AmmoContainer.HasEnaughAmmo(AmmoType.Bullets, 1))
            {
                if (!_weaponStateMachine.TryFire()) return;
                BulletObjectFactory.CreateBullet(position, degAngle, BulletPrototype, (int)Layers.PlayerBullets, _dynamicGameObjects.transform);
                AmmoContainer.RemoveAmmo(AmmoType.Bullets, 1);
            }
        }
    }
}
