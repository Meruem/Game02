namespace Assets.Scripts.Messages
{
    public class AmmoChangedMessage : IMessage
    {
        public int NewAmount { get; private set; }

        public AmmoChangedMessage(int newAmount)
        {
            NewAmount = newAmount;
        }
    }
}
