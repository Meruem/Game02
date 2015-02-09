using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public abstract class WeaponBase
    {
        public abstract string Name { get; }

        public virtual void Fire(Vector2 position, float degAngle)
        {
        }
    }
}
