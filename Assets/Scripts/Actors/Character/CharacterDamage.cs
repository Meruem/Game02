using Assets.Scripts.Messages;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Actors.Stats;

namespace Assets.Scripts.Actors.Character
{
    public class CharacterDamage
    {
        private readonly List<int> _blockedWeaponIds = new List<int>();
        private readonly List<WeaponHitMessage> _unresolvedHitMessages = new List<WeaponHitMessage>();
        private float _nextReset;
        private float _timeDeltaWait = 0.01f;
        private readonly Stats.Stats _stats;

        public CharacterDamage(Stats.Stats stats)
        {
            _stats = stats;
        }

        public void ApplyDamage()
        {
            if (_nextReset > 0 && Time.time > _nextReset)
            {
                _blockedWeaponIds.Clear();
                foreach (var message in _unresolvedHitMessages)
                {
                    _stats.AddAmount(StatsEnum.Health, -message.Damage);
                    _stats.AddAmount(StatsEnum.Stability, -message.StabilityDamage);
                }
                _unresolvedHitMessages.Clear();
                _nextReset = 0;
            }
        }

        public void RegisterNewBlockedDamageSource(int id)
        {
            _blockedWeaponIds.Add(id);
            _unresolvedHitMessages.RemoveAll(m => m.Weapon.GetInstanceID() == id);
            _nextReset = Time.time + _timeDeltaWait;
        }

        public void RegisterNewDamage(WeaponHitMessage weaponHitMessage)
        {
            if (_blockedWeaponIds.Contains(weaponHitMessage.Weapon.GetInstanceID())) return;
            _unresolvedHitMessages.Add(weaponHitMessage);
            _nextReset = Time.time + _timeDeltaWait;
        }
    }
}
