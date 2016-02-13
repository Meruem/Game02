namespace Assets.Scripts.Messages
{
    public class FireMessage : IMessage
    {
        public bool IsSecondary { get; private set; }

        public FireMessage(bool isSecondary = false)
        {
            IsSecondary = isSecondary;
        }
    }
}
