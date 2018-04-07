namespace Assets.Scripts.Messages
{
    public class StaggeredMessage : IMessage
    {
        public StaggeredMessage(float time)
        {
            Time = time;
        }

        public float Time { get; private set; }
    }
}
