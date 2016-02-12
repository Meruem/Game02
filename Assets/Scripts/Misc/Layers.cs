using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public enum LayerName
    {
        Default,
        Player,
        Monster,
        PlayerBullets,
        MonsterBullets,
        ShadowLayer
    }

    public static class Layers
    {
        private static readonly Dictionary<LayerName, int> _layerMapping = new Dictionary<LayerName, int>();

        static Layers()
        {
            foreach (LayerName layerName in Enum.GetValues(typeof(LayerName)))
            {
                _layerMapping[layerName] = LayerMask.NameToLayer(layerName.ToString());
            }
        }

        public static int GetLayer(LayerName name)
        {
            if (!_layerMapping.ContainsKey(name))
            {
                Debug.LogErrorFormat("Layer with name {0} is not defined.", name.ToString());
                throw new Exception("Layer not found");
            }

            return _layerMapping[name];
        }
    }
}
