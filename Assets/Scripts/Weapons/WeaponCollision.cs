﻿using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class WeaponCollision : MonoBehaviour
    {
        public WeaponScript WeaponStats;

        void OnTriggerEnter2D(Collider2D other)
        {
            // prevent multihits of same target
            // Debug.LogFormat("Collided with {0}", other.GetInstanceID());
            if (WeaponStats == null || WeaponStats.AlreadyHitTargets == null || other == null)
            {
                Debug.Break();
            }

            if (WeaponStats.AlreadyHitTargets.Contains(other.GetInstanceID())) return;
            WeaponStats.AlreadyHitTargets.Add(other.GetInstanceID());
            other.GetPubSub().PublishBubbleMessage(new WeaponHitMessage(WeaponStats.WeaponDamage, gameObject)
            {
                AfterBlockEnergyDamage = WeaponStats.AfterBlockEnergyDamage,
                StabilityDamage = WeaponStats.StabilityDamage
            },
            true);
        }
    }
}
