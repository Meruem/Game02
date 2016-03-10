namespace Assets.Scripts.Messages
{
    public class StaggerMessage : IMessage
    {
        public StaggerMessage(float time)
        {
            Time = time;
        }

        public float Time { get; private set; }
    }
}
