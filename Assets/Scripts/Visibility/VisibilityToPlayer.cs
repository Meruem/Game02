using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Visibility
{
    [RequireComponent(typeof (Renderer))]
    public class VisibilityToPlayer : MonoBehaviour
    {
        public float MaxDistance;

        private Renderer _renderer;
        private Transform _mainPlayer;
        private int _layerMask;

        public void Start()
        {
            _renderer = GetComponent<Renderer>();
            _mainPlayer = GameObjectEx.FindGameObjectWithTag(GameObjectTags.Player).transform;
            var shadowLayer = Layers.GetLayer(LayerName.ShadowLayer);
            _layerMask = (1 << shadowLayer) | (1 << Layers.GetLayer(LayerName.Player));
        }

        public void Update()
        {
            var outside = (_mainPlayer.position - transform.position).magnitude > MaxDistance;
            if (outside)
            {
                _renderer.enabled = false;
            }
            else
            {
                var hits = Physics2D.Raycast(transform.position, _mainPlayer.position - transform.position, MaxDistance,
                    _layerMask);
                _renderer.enabled = hits.collider == null || hits.collider.transform == _mainPlayer;
            }
        }
    }
}
