namespace Assets.Scripts.Messages
{
    public class FireInputMessage : IMessage
    {
        public bool IsSecondary { get; private set; }

        public FireInputMessage(bool isSecondary = false)
        {
            IsSecondary = isSecondary;
        }
    }
}
