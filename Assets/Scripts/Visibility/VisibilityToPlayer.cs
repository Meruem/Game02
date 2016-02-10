using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Visibility
{
    public class VisibilityToPlayer : MonoBehaviour
    {
        public float MaxDistance;
        public GameObject Target;

        private Renderer _renderer;
        private Transform _mainPlayer;
        private int _layerMask;

        void Start()
        {
            _renderer = GetComponent<Renderer>();
            var shadowLayer = LayerMask.NameToLayer("ShadowLayer");
            _layerMask = (1 << shadowLayer) | (1 << (int)Layers.Player);
        }

        void Update()
        {
            if (_mainPlayer == null)
            {
                _mainPlayer = Player.MainPlayer == null ? null : Player.MainPlayer.transform;
            }

            if (_mainPlayer != null)
            {
                var outside = (_mainPlayer.position - transform.position).magnitude > MaxDistance;
                if (outside)
                {
                    _renderer.enabled = false;
                }
                else
                {
                    var hits = Physics2D.Raycast(transform.position, _mainPlayer.position - transform.position, MaxDistance, _layerMask);
                    _renderer.enabled = hits.collider == null || hits.collider.transform == _mainPlayer;
                }
            }
        }
    }
}
