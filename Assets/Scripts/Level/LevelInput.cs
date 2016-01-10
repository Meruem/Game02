using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(LevelGeneratorScript))]
    class LevelInput : MonoBehaviour
    {
        private LevelGeneratorScript _level;

        void Awake()
        {
            _level = GetComponent<LevelGeneratorScript>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _level.Restart();
            }
        }
    }
}
