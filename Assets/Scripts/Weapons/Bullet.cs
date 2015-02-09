using UnityEngine;

namespace Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        public int Speed = 10;
        public int Damage = 3;

        void OnCollisionEnter2D(Collision2D coll)
        {
            var damageTrigger = coll.gameObject.GetComponent<TakeDamageTrigger>();
            if (damageTrigger != null)
            {
                damageTrigger.TakeDamage(Damage);
            }

            Destroy(gameObject);
        }
    }
}