using System.Collections;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Actors.Movement
{
    public class ActorHit : MonoBehaviour
    {
        public float HitTime;
        private Animator _animator;

        public void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null) Debug.Log("ActorHit requires animator component in children");
        }

        public void Start()
        {
            this.GetPubSub().SubscribeInContext<WeaponHitMessage>(m => HandleWeaponHit((WeaponHitMessage)m));
        }

        private void HandleWeaponHit(WeaponHitMessage weaponHitMessage)
        {
            StartCoroutine(IsHitForTime());
        }

        private IEnumerator IsHitForTime()
        { 
            _animator.SetBool("IsHit", true);
            yield return new  WaitForSeconds(HitTime);
            _animator.SetBool("IsHit", false);
        }
    }
}