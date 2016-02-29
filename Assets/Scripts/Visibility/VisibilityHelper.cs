using UnityEngine;

namespace Assets.Scripts.Visibility
{
    class VisibilityHelper
    {
        public static bool CheckVisibility(Transform t1, Transform t2, float maxDistance, int layerMask)
        {
            var outside = (t1.position - t2.position).magnitude > maxDistance;
            if (outside) return false;

            var hits = Physics2D.Raycast(t1.position, t2.position - t1.position, maxDistance, layerMask);
            return hits.collider == null || hits.collider.transform == t2;
        }
    }
}
