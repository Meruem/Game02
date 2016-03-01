﻿using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class PlayerBasicGun : MonoBehaviour, IWeapon
    {
        public AmmoContainer AmmoContainer; 
        public float WeaponCooldown;    // forwardTime until next shot is ready
        public BulletPrototype BulletPrototype;

        private WeaponStateMachine _weaponStateMachine;
        private GameObject _dynamicGameObjects;
        private int _id;

        public void Awake()
        {
            _id = gameObject.GetInstanceID();
            _weaponStateMachine = new WeaponStateMachine(WeaponCooldown, 0);
            _dynamicGameObjects = GameObjectEx.Find(GameObjectNames.DynamicObjects);
            this.GetPubSub().SubscribeInContext<FireMessage>(m =>
            {
                if (!((FireMessage) m).IsSecondary) Fire();
            });
        }

        public void Fire()
        {
            Fire(transform.position, transform.rotation.eulerAngles.z);
            this.GetPubSub().PublishMessageInContext(new AttackEndedMessage());
        }

        public int Id
        {
            get { return _id; }
        }

        private void Fire(Vector2 position, float degAngle)
        {
            if (AmmoContainer == null)
            {
                Debug.LogWarning("Ammo container not found for this gun!");
            }
            else if (AmmoContainer.HasEnaughAmmo(AmmoType.Bullets, 1))
            {
                if (!_weaponStateMachine.TryFire()) return;
                BulletObjectFactory.CreateBullet(position, degAngle, BulletPrototype, Layers.GetLayer(LayerName.PlayerBullets), _dynamicGameObjects.transform);
                AmmoContainer.RemoveAmmo(AmmoType.Bullets, 1);
            }
        }
    }
}
