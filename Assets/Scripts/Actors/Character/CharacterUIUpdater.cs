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
            switch (statChangedMessage.Stat)
            {
                case StatsEnum.Health:
                    this.GetPubSub().PublishMessageGlobal(new HealthChangedMessage(statChangedMessage.NewValue));
                    return;

                case StatsEnum.Energy:
                    this.GetPubSub().PublishMessageGlobal(new EnergyChangedMessage(statChangedMessage.NewValue));
                    return;

                case StatsEnum.Bullets:
                    this.GetPubSub().PublishMessageGlobal(new AmmoChangedMessage(statChangedMessage.NewValue));
                    return;
            }
        }
    }
}
