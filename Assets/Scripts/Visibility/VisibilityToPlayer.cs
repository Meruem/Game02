using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Visibility
{
    public class VisibilityToPlayer : MonoBehaviour
    {
        public float MaxDistance;

        private Visibility[] _visibilities;
        private Transform _mainPlayer;
        private int _layerMask;

        public void Start()
        {
            _visibilities = GetComponentsInChildren<Visibility>();
            _mainPlayer = GameObjectEx.FindGameObjectWithTag(GameObjectTags.Player).transform;
            var shadowLayer = Layers.GetLayer(LayerName.ShadowLayer);
            _layerMask = (1 << shadowLayer) | (1 << Layers.GetLayer(LayerName.Player));
        }

        public void Update()
        {
            var outside = (_mainPlayer.position - transform.position).magnitude > MaxDistance;
            if (outside)
            {
                _visibilities.ForEach(v => v.IsVisible = false);
            }
            else
            {
                var hits = Physics2D.Raycast(transform.position, _mainPlayer.position - transform.position, MaxDistance,
                    _layerMask);
                _visibilities.ForEach(v => v.IsVisible = hits.collider == null || hits.collider.transform == _mainPlayer);
            }
        }
    }
}
