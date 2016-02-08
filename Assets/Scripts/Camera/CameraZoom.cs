using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class CameraZoom : MonoBehaviour
    {
        public int Delta = 3;

        private Camera _camera;

        void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        public void FixedUpdate()
        {
            var delta = Input.GetAxis("Mouse ScrollWheel");
            if (delta > 0)
            {
                _camera.orthographicSize += Delta;
            }
            else if (delta < 0)
            {
                _camera.orthographicSize -= Delta;
            }

        }
    }
}