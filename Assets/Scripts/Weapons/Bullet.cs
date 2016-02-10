using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        public int Speed = 10;
        public int Damage = 3;

        void OnCollisionEnter2D(Collision2D coll)
        {
            coll.gameObject.GetPubSub().PublishMessageInContext(new TakeDamageMessage(Damage));
            Destroy(gameObject);
        }
    }
}