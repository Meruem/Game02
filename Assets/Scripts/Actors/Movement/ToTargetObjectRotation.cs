using UnityEngine;

namespace Assets.Scripts.Actors.Movement
{
    public class ToTargetObjectRotation : MonoBehaviour
    {
        public Transform Target;

        public void Update()
        {
            if (Target != null)
            {
                var relativePos = Target.position - transform.position;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, relativePos);
            }
        }
    }
}
