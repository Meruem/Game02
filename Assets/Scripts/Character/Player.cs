using UnityEngine;

namespace Assets.Scripts.Character
{
    public class Player : MonoBehaviour
    {
        public static Player MainPlayer;

        void Start()
        {
            MainPlayer = this;
        }
    }
}
