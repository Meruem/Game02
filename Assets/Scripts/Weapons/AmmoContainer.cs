using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    [Serializable]
    public class AmmoInfo
    {
        public AmmoType Type;
        public int MaxAmount;
        public int StartingAmount;
    }
    
    public class AmmoContainer : MonoBehaviour
    {
        private readonly Dictionary<AmmoType, int> _amounts;
        public AmmoInfo[] Ammo;

        public void AddAmmo(AmmoType type, int amount)
        {
            var max = Ammo.First(a => a.Type == type).MaxAmount;

            int newAmount = amount;

            if (_amounts.ContainsKey(type))
            {
                newAmount = amount + _amounts[type];
            }

            _amounts[type] = Math.Min(newAmount, max);
            if (newAmount != max)
            {
                PubSub.GlobalPubSub.Publish(new AmmoChangedMessage(type, newAmount));
            }
        }

        public int AmmoAmmount(AmmoType type)
        {
            return _amounts.ContainsKey(type) ? _amounts[type] : 0;
        }

        public bool HasEnaughAmmo(AmmoType type, int amount)
        {
            return _amounts.ContainsKey(type) && _amounts[type] >= amount;
        }

        public void RemoveAmmo(AmmoType type, int amount)
        {
            if (!HasEnaughAmmo(type, amount)) return;

            _amounts[type] -= amount;
            PubSub.GlobalPubSub.Publish(new AmmoChangedMessage(type, _amounts[type]));
        }
    }
}
