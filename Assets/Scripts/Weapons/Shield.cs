using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class Shield : MonoBehaviour
    {
        public float ShieldEnergyDamageRatio = 0.9f;

        private PolygonCollider2D[] _colliders;
        private SpriteRenderer[] _renderers;

        public void Awake()
        {
            _colliders = GetComponentsInChildren<PolygonCollider2D>();
            _renderers = GetComponentsInChildren<SpriteRenderer>();
        }

        public void Start()
        {
            this.GetPubSub().SubscribeInContext<ShieldChangeMessage>(m => HandleShieldChange((ShieldChangeMessage)m));
            this.GetPubSub().Subscribe<WeaponHitMessage>(m => HandleTakeDamage((WeaponHitMessage)m));
        }

        private void HandleTakeDamage(WeaponHitMessage message)
        {
            Debug.LogFormat("Shield hit");
            message.Weapon.GetPubSub().PublishMessageInContext(new WeaponBlockedMessage()); // notify weapon object
            this.GetPubSub().PublishMessageInContext(new ShieldHitMessage(message.Weapon)
            {
                EnergyDamage = (int)(message.AfterBlockEnergyDamage * ShieldEnergyDamageRatio),
                OriginalDamage = message.Damage
            }); // notify parent object
        }

        private void HandleShieldChange(ShieldChangeMessage shieldChangeMessage)
        {
            _colliders.ForEach(c => c.enabled = shieldChangeMessage.ShieldUp);
            _renderers.ForEach(r => r.enabled = shieldChangeMessage.ShieldUp);
        }
    }
}
