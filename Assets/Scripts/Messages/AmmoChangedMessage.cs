using Assets.Scripts.Weapons;

namespace Assets.Scripts.Messages
{
    public class AmmoChangedMessage : IMessage
    {
        public int NewAmount { get; private set; }
        public AmmoType Type { get; private set; }

        public AmmoChangedMessage(AmmoType type, int newAmount)
        {
            NewAmount = newAmount;
            Type = type;
        }
    }
}
