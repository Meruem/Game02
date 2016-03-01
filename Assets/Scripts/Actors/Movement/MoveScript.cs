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
    public class MoveScript : MonoBehaviour, IMoveScript
    {
        public float MaxSpeed = 5.0f;
        
        private Animator _animator;
        private Rigidbody2D _cachedRigidBody2D;

        private bool _isInForcedMovement;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _cachedRigidBody2D = GetComponent<Rigidbody2D>();
            this.GetPubSub().SubscribeInContext<ForceMovementMessage>(m => ForceMove((ForceMovementMessage)m));
        }

        public void MoveNormal(Vector2 direction, float speed)
        {
            if (_isInForcedMovement) return;
            Move(direction, speed);
        }

        public void MoveMaxSpeed(Vector2 direction)
        {
            MoveNormal(direction, MaxSpeed);
        }

        private void ForceMove(ForceMovementMessage forceMovementMessage)
        {
            if (forceMovementMessage.AllowOtherMovement) return;
            StartCoroutine(ForceMoveCoroutine(forceMovementMessage.Direction, forceMovementMessage.Speed, forceMovementMessage.ForwardTime));
        }

        private IEnumerator ForceMoveCoroutine(Vector2 direction, float speed, float time)
        {
            _isInForcedMovement = true;
            Move(direction, speed);
            yield return new WaitForSeconds(time);
            Move(Vector2.zero, 0);
            _isInForcedMovement = false;
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