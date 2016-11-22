using Assets.Scripts.Actors.Stats;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Game02.Assets.Scripts.Messages;
using UnityEngine;

namespace Assets.Scripts.Actors.Character
{
    public class CharacterUIUpdater : MonoBehaviour
    {
        public void Awake()
        {
            this.GetPubSub().SubscribeInContext<StatChangedMessage>(m => HandleStatChangedMesssage((StatChangedMessage)m));
        }

        private void HandleStatChangedMesssage(StatChangedMessage statChangedMessage)
        {
            if (statChangedMessage.Stat == StatsEnum.Health)
            {
                this.GetPubSub().PublishMessageGlobal(new HealthChangedMessage(statChangedMessage.NewValue));
                return;
            }

            if (statChangedMessage.Stat == StatsEnum.Energy)
            {
                this.GetPubSub().PublishMessageGlobal(new EnergyChangedMessage(statChangedMessage.NewValue));
            }

            if (statChangedMessage.Stat == StatsEnum.Bullets)
            {
                this.GetPubSub().PublishMessageGlobal(new AmmoChangedMessage(statChangedMessage.NewValue));
            }
        }
    }
}
