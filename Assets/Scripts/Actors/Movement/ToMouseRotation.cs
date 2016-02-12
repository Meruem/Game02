using System.Collections;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts
{
    class ToMouseRotation : MonoBehaviour
    {
        public float RotationAdjustment = -90;

        private bool _isInForcedMovement;

        public void Start()
        {
            this.GetPubSub().SubscribeInContext<ForceMovementMessage>(m => OnForcedMovement((ForceMovementMessage)m));
        }

        private void OnForcedMovement(ForceMovementMessage message)
        {
            StartCoroutine(OnForcedMovementCourutine(message));
        }

        private IEnumerator OnForcedMovementCourutine(ForceMovementMessage message)
        {
            _isInForcedMovement = true;
            yield return new WaitForSeconds(message.ForwardTime + message.StopTime);
            _isInForcedMovement = false;
        }

        void Update()
        {
            if (_isInForcedMovement) return;

            var difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize();
            var angle = Mathf.Atan2(difference.y, difference.x);

            transform.rotation = Quaternion.Euler(0f, 0f, angle*Mathf.Rad2Deg + RotationAdjustment);
        }
    }
}
