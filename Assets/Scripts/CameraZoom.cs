using UnityEngine;

namespace Assets.Scripts
{
    public class CameraZoom : MonoBehaviour
    {
        public int Delta = 3;

        public void FixedUpdate()
        {
            var delta = Input.GetAxis("Mouse ScrollWheel");
            if (delta > 0)
            {
                Camera.main.orthographicSize += Delta;
            }
            else if (delta < 0)
            {
                Camera.main.orthographicSize -= Delta;
            }

        }
    }
}