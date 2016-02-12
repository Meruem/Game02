using UnityEngine;

namespace Assets.Scripts.Misc
{
    public static class PubSubExtension
    {
        public static PubSub GetPubSub(this GameObject go)
        {
            var pubSub = go.GetComponent<PubSub>();
            if (pubSub == null)
            {
                pubSub = go.AddComponent<PubSub>();
            }

            return pubSub;
        }

        public static PubSub GetPubSub(this MonoBehaviour mb)
        {
            return mb.gameObject.GetPubSub();
        }

        public static PubSub GetPubSub(this Component c)
        {
            return c.gameObject.GetPubSub();
        }
    }
}
