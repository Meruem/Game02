using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class TakeDamageTrigger : MonoBehaviour
    {
        public event Action<int> OnTakeDamage;

        public void TakeDamage(int damage)
        {
            var @event = OnTakeDamage;
            if (@event != null)
            {
                @event(damage);
            }
        }
    }
}