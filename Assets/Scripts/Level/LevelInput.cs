using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts
{
    class LevelInput : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                this.GetPubSub().PublishMessageGlobal(new RestartLevelMessage());
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}
