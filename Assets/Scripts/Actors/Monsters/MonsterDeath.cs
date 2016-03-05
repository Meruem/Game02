using Assets.Scripts.Actors.Stats;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Actors.Monsters
{
    public class MonsterDeath : MonoBehaviour
    {
        public void Awake()
        {
            this.GetPubSub().SubscribeInContext<StatChangedMessage>(m => HandleStatChangedMessage((StatChangedMessage)m), m => Filter((StatChangedMessage)m));
        }

        private bool Filter(StatChangedMessage statChangedMessage)
        {
            return statChangedMessage.Stat == StatsEnum.Health && statChangedMessage.NewValue <= 0;
        }

        private void HandleStatChangedMessage(StatChangedMessage statChangedMessage)
        {
            Destroy(gameObject);
        }
    }
}
