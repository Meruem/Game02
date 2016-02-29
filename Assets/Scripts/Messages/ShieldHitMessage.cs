using UnityEngine;

namespace Assets.Scripts.Messages
{
    public class ShieldHitMessage : IMessage
    {
        public GameObject Weapon { get; private set; }

        public ShieldHitMessage(GameObject weapon)
        {
            Weapon = weapon;
        }
    }
}
