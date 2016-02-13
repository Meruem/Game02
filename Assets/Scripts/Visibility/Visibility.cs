using UnityEngine;

namespace Assets.Scripts.Visibility
{
    public class Visibility : MonoBehaviour
    {
        public bool IsVisible = true;
        public bool IsVisibleByAnimation = true;

        private Renderer[] _renderers;

        public void Start()
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }

        public void Update()
        {
            if (_renderers == null) return;

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].enabled = IsVisible && IsVisibleByAnimation;
            }
        }
    }
}
