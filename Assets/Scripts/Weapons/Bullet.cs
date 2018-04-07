using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        public int Speed = 10;
        public int Damage = 3;

        void OnCollisionEnter2D(Collision2D coll)
        {
            coll.collider.gameObject.GetPubSub().PublishBubbleMessage(new WeaponHitMessage(Damage, gameObject), true);
            Destroy(gameObject);
        }

        public static Transform CreateBullet(Vector2 position, float degAngle, BulletPrototype bulletPrototype)
        {
            var vector = Quaternion.AngleAxis(degAngle, Vector3.forward)*Vector3.up;

            var bullet =
                (Transform)
                    Object.Instantiate(bulletPrototype.Prefab, position,
                        Quaternion.Euler(0, 0, degAngle + 90));
            bullet.gameObject.layer = Layers.GetLayer(LayerName.Player);
            var bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Damage = bulletPrototype.Damage;
            bulletScript.Speed = bulletPrototype.Speed;
            bullet.GetComponent<Rigidbody2D>().velocity = vector*bulletPrototype.Speed;

            return bullet;
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