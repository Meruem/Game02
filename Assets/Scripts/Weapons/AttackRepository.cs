using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class AttackRepository : MonoBehaviour
    {
        public List<Transform> Attacks;

        private Dictionary<string, Transform> _attacksDictionary;

        public Transform GetAttack(string attackName)
        {
            if (_attacksDictionary == null)
            {
                _attacksDictionary = Attacks.ToDictionary(a => a.name);
            }

            return _attacksDictionary.ContainsKey(attackName) ? _attacksDictionary[attackName] : null;
        }
    }
}
