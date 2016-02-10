using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Actors.Monsters
{
    public class MonsterDeath : MonoBehaviour
    {
        public void Awake()
        {
            this.GetPubSub().Subscribe<DeathMessage>(m => { Destroy(gameObject); });
        }
    }
}
