using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveScript : MonoBehaviour
    {
        public float MaxSpeed = 5.0f;
        
        private Animator _animator;
        private Rigidbody2D _cachedRigidBody2D;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _cachedRigidBody2D = GetComponent<Rigidbody2D>();
            this.GetPubSub().SubscribeInContext<MoveInDirectionMessage>(m => Move(((MoveInDirectionMessage)m).Direction));
        }
        
        public void Move(Vector2 movement)
        {
            if (_cachedRigidBody2D == null) return;

            //move the rigid body, which is part of the physics system
            //This ensures smooth movement.
            _cachedRigidBody2D.velocity = new Vector2(movement.x * MaxSpeed, movement.y * MaxSpeed);

            //take the absolute value and add, because x or y 
            //may be negative and potentially cancel eachother out.
            float speed = Mathf.Abs(movement.x) + Mathf.Abs(movement.y);

            //set the speed variable in the animation component to ensure proper state.
            _animator.SetFloat("Speed", speed);
        }         
    }
}