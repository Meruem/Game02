using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveScript : MonoBehaviour
    {
        public float MaxSpeed = 5.0f;
        public int FacingAngleAdjustment;
        
        private Animator _animator;
        private Rigidbody2D _cachedRigidBody2D;
        public Transform Body;

        private void Start()
        {
            _animator = Body.GetComponent<Animator>();
            _cachedRigidBody2D = GetComponent<Rigidbody2D>();
        }
        
        public void Move(Vector2 movement)
        {
            //move the rigid body, which is part of the physics system
            //This ensures smooth movement.
            _cachedRigidBody2D.velocity = new Vector2(movement.x * MaxSpeed, movement.y * MaxSpeed);

            //take the absolute value and add, because x or y 
            //may be negative and potentially cancel eachother out.
            float speed = Mathf.Abs(movement.x) + Mathf.Abs(movement.y);

            //set the speed variable in the animation component to ensure proper state.
            _animator.SetFloat("Speed", speed);

            //convert the vector into a radian angle, 
            //convert to degrees and then adjust for the 
            //spider's starting orientation
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg + FacingAngleAdjustment;

            //don't rotate if we don't need to.
            if (speed > 0.0f)
            {
                //rotate by angle around the z axis.
                Body.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
            }
        }         
    }
}