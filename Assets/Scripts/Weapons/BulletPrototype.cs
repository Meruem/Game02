using System;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    [Serializable]
    public class BulletPrototype
    {
        public int Speed;
        public int Damage;
        public Transform Prefab;
    }
}
