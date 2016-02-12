using System;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Game02.Assets.Scripts.Messages;
using UnityEngine;

namespace Assets.Scripts.Actors
{
    public class Health : MonoBehaviour
    {
        public int StartingLives;
        public int Lives;
        public bool NotifyHealthChanged = false;

        public void Awake()
        {
            this.GetPubSub().Subscribe<TakeDamageMessage>(m =>
            {
                OnTakeDamage(((TakeDamageMessage) m).Damage); 
            });
        }

        public void Start()
        {
            Lives = StartingLives;
        }

        private void OnTakeDamage(int damage)
        {
            Lives -= damage;
            if (NotifyHealthChanged)
            {
                PubSub.GlobalPubSub.PublishMessage(new HealthChangedMessage(Lives));
            }

            if (Lives <= 0)
            {
                this.GetPubSub().PublishMessageInContext(new DeathMessage());
            }
        }
    }
}
