using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    class ToMoveDirectionRotation : MonoBehaviour
    {
        public float RotationAdjustment = -90;
        private Rigidbody2D _cachedRigidBody2D;


        void Update()
        {
            _cachedRigidBody2D = GetComponent<Rigidbody2D>();
            var movement = _cachedRigidBody2D.velocity;
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg + RotationAdjustment;

            float speed = Mathf.Abs(movement.x) + Mathf.Abs(movement.y);

            if (speed > 0.0f)
            {
                //rotate by angle around the z axis.
                transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
            }
        }
    }
}
