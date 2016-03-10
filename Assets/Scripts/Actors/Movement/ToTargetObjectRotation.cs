using System.Collections;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Actors.Movement
{
    public class ToTargetObjectRotation : MonoBehaviour
    {
        public Transform Target;

        private bool _isInForcedMovement;

        public void Start()
        {
            this.GetPubSub().SubscribeInContext<ForceMovementMessage>(m => OnForcedMovement((ForceMovementMessage)m));
        }

        private void OnForcedMovement(ForceMovementMessage message)
        {
            if (!message.AllowOtherMovement)
            {
                _isInForcedMovement = true;
                this.StartAfterTime(() => { _isInForcedMovement = false; }, message.ForwardTime);
            }
        }

        public void Update()
        {
            if (_isInForcedMovement) return;
            if (Target == null) return;

            var relativePos = Target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, relativePos);
        }
    }
}
