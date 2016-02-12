using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public enum GameObjectNames
    {
        DynamicObjects,
        Tiles,
        Monsters,
        Colliders,
        Collider,
        GlobalPubSub,
        PubSubSettings
    }

    public enum GameObjectTags
    {
        Player
    }

    public static class GameObjectNameMapping
    {
        private static readonly Dictionary<GameObjectNames, string> _overrideMappings = new Dictionary<GameObjectNames, string>
        {
            {GameObjectNames.DynamicObjects, "Dynamic Objects"},
            {GameObjectNames.Collider, "@Collider"},
        };

        public static string GetObjectName(GameObjectNames name)
        {
            if (!_overrideMappings.ContainsKey(name))
            {
                return name.ToString();
            }

            return _overrideMappings[name];
        }
    }

    public static class GameObjectEx
    {
        public static GameObject Find(GameObjectNames name)
        {
            return GameObject.Find(GameObjectNameMapping.GetObjectName(name));
        }

        public static GameObject FindGameObjectWithTag(GameObjectTags tag)
        {
            return GameObject.FindGameObjectWithTag(tag.ToString());
        }
    }
}
