using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(PolygonCollider2D))]
    class Weapon : MonoBehaviour
    {
        private bool _isFiring = false;
        private PolygonCollider2D _collider;

        void Start()
        {
            _collider = GetComponent<PolygonCollider2D>();
        }

        void Update()
        {
            _isFiring = Input.GetMouseButtonDown(1);
            _collider.enabled = _isFiring;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isFiring) return;

            var takeDamage = other.GetComponent<TakeDamageTrigger>();
            if (takeDamage != null)
            {
                takeDamage.TakeDamage(1);
            }
        }
    }
}
