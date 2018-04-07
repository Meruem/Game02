using System;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    [CreateAssetMenu(menuName="Attacks/Bullets")]
    public class BulletPrototype : ScriptableObject
    {
        public int Speed;
        public int Damage;
        public Transform Prefab;
    }
}
