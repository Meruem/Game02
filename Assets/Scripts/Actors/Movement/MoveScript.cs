using System.Collections;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Listens for MoveInDirection and ForceMovement messages and moves attached rigid body accordingly.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveScript : MonoBehaviour
    {
        public float MaxSpeed = 5.0f;
        
        private Animator _animator;
        private Rigidbody2D _cachedRigidBody2D;

        private bool _isInForcedMovement;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _cachedRigidBody2D = GetComponent<Rigidbody2D>();
            this.GetPubSub().SubscribeInContext<MoveInDirectionMessage>(m => MoveNormal((MoveInDirectionMessage)m));
            this.GetPubSub().SubscribeInContext<ForceMovementMessage>(m => ForceMove((ForceMovementMessage)m));
        }

        private void ForceMove(ForceMovementMessage forceMovementMessage)
        {
            if (forceMovementMessage.AllowOtherMovement) return;
            StartCoroutine(ForceMoveCoroutine(forceMovementMessage));
        }

        private IEnumerator ForceMoveCoroutine(ForceMovementMessage message)
        {
            _isInForcedMovement = true;
            Move(message.Direction, message.Speed);
            yield return new WaitForSeconds(message.ForwardTime);
            Move(Vector2.zero, 0);
            _isInForcedMovement = false;
        }

        private void MoveNormal(MoveInDirectionMessage message)
        {
            if (_isInForcedMovement) return;
            Move(message.Direction, message.UseMaxSpeed ? MaxSpeed : message.Speed);
        }

        private void Move(Vector2 movement, float speed)
        {
            if (_cachedRigidBody2D == null) return;

            _cachedRigidBody2D.velocity = new Vector2(movement.x * speed, movement.y * speed);

            float aniSpeed = Mathf.Abs(movement.x) + Mathf.Abs(movement.y);

            _animator.SetFloat("Speed", aniSpeed);
        }         
    }
}