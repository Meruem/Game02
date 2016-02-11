namespace Game02.Assets.Scripts.Messages
{
    public class HealthChangedMessage : IMessage
    {
        public int NewHealth { get; private set; }
        
        public HealthChangedMessage(int newHealth)
        {
            NewHealth = newHealth;
        }
    }
}
