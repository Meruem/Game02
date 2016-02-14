namespace Assets.Scripts.Messages
{
    public class ShieldChangeMessage : IMessage
    {
        public bool ShieldUp { get; private set; }

        public ShieldChangeMessage(bool shieldUp)
        {
            ShieldUp = shieldUp;
        }
    }
}
