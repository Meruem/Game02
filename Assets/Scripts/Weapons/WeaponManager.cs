using System.Collections.Generic;
using Assets.Scripts.Actors.Stats;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        public Stats Stats;

        private IList<IAttack> _weapons = new List<IAttack>();
        private IAttack _primaryAttack;
        private IAttack _secondaryAttack;

        public void Awake()
        {
            _weapons = GetComponentsInChildren<IAttack>();
            if (_weapons != null && _weapons.Count > 0)
            {
                _primaryAttack = _weapons[0];
                _secondaryAttack = _weapons.Count > 1 ? _weapons[1] : _weapons[0];
            }
        }

        public void ChangePrimaryWeapon(int weaponNumber)
        {
            if (!CheckWeaponCount(weaponNumber)) return;
            _primaryAttack = _weapons[weaponNumber];
        }

        public void ChangeSecondaryWeapon(int weaponNumber)
        {
            if (!CheckWeaponCount(weaponNumber)) return;
            _secondaryAttack = _weapons[weaponNumber];
        }

        public void FirePrimary()
        {
            if (_primaryAttack == null)
            {
                Debug.Log("No primary weapon selected");
                return;
            }

            FireAttack(_primaryAttack);
        }

        public void FireAttack(IAttack attack)
        {
            if (Stats == null && attack.RequiredEnergy > 0)
            {
                Debug.Log("Weapon requires energy to use, but no energy component attached.");
                return;
            }

            if (Stats != null && !Stats.HasEnaugh(StatsEnum.Energy, attack.RequiredEnergy))
            {
                Debug.Log("Not enaugh energy.");
                return;
            }

            attack.Fire();
            if (Stats != null)
            {
                Stats.AddAmount(StatsEnum.Energy, -attack.RequiredEnergy);
            }
        }

        public void FireSecondary()
        {
            if (_secondaryAttack == null)
            {
                Debug.Log("No secondary weapon selected");
                return;
            }

            FireAttack(_secondaryAttack);
        }

        private bool CheckWeaponCount(int weaponNumber)
        {
            if (_weapons == null || weaponNumber > _weapons.Count)
            {
                Debug.LogFormat("Weapon number {0} not attached.", weaponNumber);
                return false;
            }

            return true;
        }
    }
}
