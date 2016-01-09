using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Misc;

namespace Assets.Scripts.Weapons
{
    public class AmmoContainer
    {
        private readonly Dictionary<AmmoType, int> _maxAmounts;
        private readonly Dictionary<AmmoType, int> _amounts;

        public AmmoContainer()
            : this(new List<Tuple<AmmoType, int>>
            {
                new Tuple<AmmoType, int>(AmmoType.Bullets, 300),
                new Tuple<AmmoType, int>(AmmoType.Energy, 40),
            })
        {
        }

        public AmmoContainer(IList<Tuple<AmmoType, int>> maxAmounts)
        {
            _maxAmounts = maxAmounts.ToDictionary(t => t.Item1, t => t.Item2);
            _amounts = new Dictionary<AmmoType, int>();
        }

        public void AddAmmo(AmmoType type, int amount)
        {
            var max = _maxAmounts[type];

            int newAmount = amount;

            if (_amounts.ContainsKey(type))
            {
                newAmount = amount + _amounts[type];
            }

            _amounts[type] = Math.Min(newAmount, max);
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
        }
    }
}
