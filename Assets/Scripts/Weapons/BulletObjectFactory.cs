using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Weapons
{
    public static class BulletObjectFactory
    {
        public static Transform CreateBullet(Vector2 position, float degAngle, BulletPrototype bulletPrototype)
        {
            var vector = Quaternion.AngleAxis(degAngle, Vector3.forward) * Vector3.up;

            try
            {
                var bullet =
                    (Transform)
                            //Object.Instantiate(Resources.Load<Transform>(bulletPrototype.PrefabName), position,
                            Object.Instantiate(bulletPrototype.Prefab, position,
                            Quaternion.Euler(0, 0, degAngle + 90));
                bullet.gameObject.layer = (int)Layers.Player;
                var bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.Damage = bulletPrototype.Damage;
                bulletScript.Speed = bulletPrototype.Speed;
                bullet.GetComponent<Rigidbody2D>().velocity = vector * bulletPrototype.Speed;

                return bullet;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public static Transform CreateBullet(Vector2 position, float degAngle, BulletPrototype bulletPrototype, int layer, Transform parent)
        {
            var bullet = CreateBullet(position, degAngle, bulletPrototype);
            bullet.gameObject.layer = layer;
            bullet.parent = parent;
            return bullet;
        }
    }
}
