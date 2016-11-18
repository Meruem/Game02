using Assets.Scripts.Actors.Stats;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class PlayerBasicGun : MonoBehaviour, IAttack
    {
        public Stats Stats; 
        public float WeaponCooldown;    // forwardTime until next shot is ready
        public BulletPrototype BulletPrototype;
        public int EnergyRequired = 10;

        private WeaponStateMachine _weaponStateMachine;
        private GameObject _dynamicGameObjects;
        private int _id;

        public void Awake()
        {
            _id = gameObject.GetInstanceID();
            _weaponStateMachine = new WeaponStateMachine(WeaponCooldown, 0);
            _dynamicGameObjects = GameObjectEx.Find(GameObjectNames.DynamicObjects);
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

        public int RequiredEnergy
        {
            get { return EnergyRequired; }
        }

        public void CancelAttack()
        {
            // do nothing
        }

        public bool CanFire { get { return _weaponStateMachine.GetState() == WeaponState.Inactive; } }

        private void Fire(Vector2 position, float degAngle)
        {
            if (Stats == null)
            {
                Debug.LogWarning("No attached 'stats' object not found for this gun!");
            }
            else if (Stats.HasEnough(StatsEnum.Bullets, 1))
            {
                if (!_weaponStateMachine.TryFire()) return;
                BulletObjectFactory.CreateBullet(position, degAngle, BulletPrototype, Layers.GetLayer(LayerName.PlayerBullets), _dynamicGameObjects.transform);
                Stats.AddAmount(StatsEnum.Bullets, -1);
            }
        }
    }
}
