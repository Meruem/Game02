using UnityEngine;

namespace Assets.Scripts.Messages
{
    public class WeaponHitMessage : IMessage
    {
        public int Damage { get; private set; }

        public int AfterBlockEnergyDamage { get; set; }

        public GameObject Weapon { get; private set; }

        public WeaponHitMessage(int damage, GameObject weapon)
        {
            Damage = damage;
            Weapon = weapon;
        }
    }
}
