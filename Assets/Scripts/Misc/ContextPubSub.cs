using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class ContextPubSub : MonoBehaviour
    {
        public void Awake()
        {
            gameObject.GetPubSub().IsContextAware = true;
        }
    }
}
