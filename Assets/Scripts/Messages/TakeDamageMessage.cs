namespace Assets.Scripts.Messages
{
    public class TakeDamageMessage : IMessage
    {
        public int Damage { get; private set; }

        public TakeDamageMessage(int damage)
        {
            Damage = damage;
        }
    }
}
