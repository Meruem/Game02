using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class WeaponCollision : MonoBehaviour
    {
        public Weapon2Script WeaponStats;

        void OnTriggerEnter2D(Collider2D other)
        {
            // prevent multihits of same target
            // Debug.LogFormat("Collided with {0}", other.GetInstanceID());
            if (WeaponStats.AlreadyHitTargets.Contains(other.GetInstanceID())) return;
            WeaponStats.AlreadyHitTargets.Add(other.GetInstanceID());
            other.GetPubSub().PublishMessageInContext(new TakeDamageMessage(WeaponStats.WeaponDamage));
        }
    }
}
