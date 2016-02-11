using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Actors
{
    public class Health : MonoBehaviour
    {
        public int StartingLives;
        public int Lives;
        
        public Action<int> OnTakeDamageAction = OnTakeDamage;

        public void Awake()
        {
            this.GetPubSub().Subscribe<TakeDamageMessage>((m => OnTakeDamageAction(((TakeDamageMessage)m).Damage)));
        }

        public void Start()
        {
            Lives = StartingLives;
        }

        private void OnTakeDamage(int damage)
        {
            Lives -= damage;
            if (Lives <= 0)
            {
                this.GetPubSub().PublishMessageInContext(new DeathMessage());
            }
        }
    }
}
