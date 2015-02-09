using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(LevelGenerator))]
    class LevelInput : MonoBehaviour
    {
        private LevelGenerator _level;

        void Awake()
        {
            _level = GetComponent<LevelGenerator>();
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
