namespace Assets.Scripts.Messages
{
    public class EnergyChangedMessage : IMessage
    {
        public int NewValue { get; private set; }

        public EnergyChangedMessage(int newValue)
        {
            NewValue = newValue;
        }
    }
}
