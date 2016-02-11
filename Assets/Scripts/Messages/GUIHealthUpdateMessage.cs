namespace Game02.Assets.Scripts.Messages
{
    public class GUIHealthUpdateMessage : IMessage
    {
        public int NewHealth { get; private set; }
        
        public GUIHealthUpdateMessage(int newHealth)
        {
            NewHealth = newHealth;
        }
    }
}
