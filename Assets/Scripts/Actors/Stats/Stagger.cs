using System.Collections;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Actors.Stats
{
    public class Stagger : MonoBehaviour
    {
        public float StaggerTime = 1;

        public bool IsStaggering { get; private set; }

        public void Start()
        {
            this.GetPubSub().SubscribeInContext<StatChangedMessage>(m => HandleStagger(), m => FilterStaggerMessage((StatChangedMessage)m));
        }

        private bool FilterStaggerMessage(StatChangedMessage statChangedMessage)
        {
            return statChangedMessage.Stat == StatsEnum.Stability && statChangedMessage.NewValue <= 0;
        }

        private void HandleStagger()
        {
            IsStaggering = true;
            this.GetPubSub().PublishMessageInContext(new StaggerMessage(StaggerTime));
            StartCoroutine(StaggerCourutine());
        }

        private IEnumerator StaggerCourutine()
        {
            yield return new WaitForSeconds(StaggerTime);
            IsStaggering = false;
        } 
    }
}
