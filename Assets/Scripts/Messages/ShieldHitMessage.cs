using UnityEngine;

namespace Assets.Scripts.Messages
{
    public class ShieldHitMessage : IMessage
    {
        public GameObject Weapon { get; private set; }
        public int EnergyDamage { get; set; }
        public int OriginalDamage { get; set; }

        public ShieldHitMessage(GameObject weapon)
        {
            Weapon = weapon;
        }
    }
}
