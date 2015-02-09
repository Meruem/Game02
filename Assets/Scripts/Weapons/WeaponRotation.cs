using UnityEngine;

namespace Assets.Scripts
{
    class WeaponRotation : MonoBehaviour
    {
        public float RotationAdjustment = -90;

        void Update()
        {
            var difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize();
            var angle = Mathf.Atan2(difference.y, difference.x);

            transform.rotation = Quaternion.Euler(0f, 0f, angle*Mathf.Rad2Deg + RotationAdjustment);
        }
    }
}
