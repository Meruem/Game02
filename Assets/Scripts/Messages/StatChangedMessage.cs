using Assets.Scripts.Actors.Stats;

namespace Assets.Scripts.Messages
{
    public class StatChangedMessage : IMessage
    {
        public StatsEnum Stat { get; private set; }
        public int NewValue { get; private set; }

        public StatChangedMessage(StatsEnum stat, int newValue)
        {
            Stat = stat;
            NewValue = newValue;
        }
    }
}
