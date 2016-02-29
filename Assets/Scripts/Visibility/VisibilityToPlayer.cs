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
            var isVisible = VisibilityHelper.CheckVisibility(transform, _mainPlayer.transform, MaxDistance, _layerMask);

            _visibilities.ForEach(v => v.IsVisible = isVisible);
        }
    }
}
