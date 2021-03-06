﻿using Assets.Scripts.Actors.Stats;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class PlayerBasicGun : MonoBehaviour, IAttack
    {
        public float WeaponCooldown;    // forwardTime until next shot is ready
        public BulletPrototype BulletPrototype;
        public int EnergyRequired = 10;
        public int BulletsRequired = 1;
        private WeaponStateMachine _weaponStateMachine;
        private GameObject _dynamicGameObjects;
        private int _id;

        public void Awake()
        {
            _id = gameObject.GetInstanceID();
            _weaponStateMachine = new WeaponStateMachine(WeaponCooldown, 0);
            _dynamicGameObjects = GameObjectEx.Find(GameObjectNames.DynamicObjects);
        }

        public void Fire(Stats stats)
        {
            Fire(transform.position, transform.rotation.eulerAngles.z, stats);
            this.GetPubSub().PublishMessageInContext(new AttackEndedMessage());
        }

        public int Id
        {
            get { return _id; }
        }

        public int RequiredEnergy
        {
            get { return EnergyRequired; }
        }

        public void CancelAttack()
        {
            // do nothing
        }

        public bool CanFire(Stats stats)
        {
            return _weaponStateMachine.GetState() == WeaponState.Inactive && stats.HasEnough(StatsEnum.Bullets, BulletsRequired); 
        }

        private void Fire(Vector2 position, float degAngle, Stats stats)
        {
            if (stats == null)
            {
                Debug.LogWarning("No attached 'stats' object not found for this gun!");
            }
            else if (stats.HasEnough(StatsEnum.Bullets, BulletsRequired))
            {
                if (!_weaponStateMachine.TryFire()) return;
                Bullet.CreateBullet(position, degAngle, BulletPrototype, Layers.GetLayer(LayerName.PlayerBullets), _dynamicGameObjects.transform);
                stats.AddAmount(StatsEnum.Bullets, -BulletsRequired);
            }
        }
    }
}
