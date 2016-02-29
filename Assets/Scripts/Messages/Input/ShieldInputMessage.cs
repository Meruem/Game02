namespace Assets.Scripts.Messages.Input
{
    public class ShieldInputMessage : IMessage
    {
        public bool ShieldUp { get; private set; }

        public ShieldInputMessage(bool shieldUp)
        {
            ShieldUp = shieldUp;
        }
    }
}
