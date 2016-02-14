using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class RootPubSub : MonoBehaviour
    {
        public void Awake()
        {
            gameObject.GetPubSub().IsRoot = true;
        }
    }
}
