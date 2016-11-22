using System.Collections.Generic;
using Assets.Scripts.Actors.Stats;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        public Stats Stats;

        public AttackRepository AttackRepository;

        public List<string> AttackNames;

        private IList<IAttack> _weapons = new List<IAttack>();
        private IAttack _primaryAttack;
        private IAttack _secondaryAttack;

        public void Start()
        {
            if (AttackNames != null && AttackRepository != null)
            {
                AttackNames.ForEach(a =>
                {
                    var attack = AttackRepository.GetAttack(a);
                    if (attack != null)
                    {
                        var attackTransform = (Transform) Instantiate(attack, Vector3.zero, Quaternion.identity);
                        attackTransform.SetParent(transform, false);
                    }
                });
            }

            _weapons = GetComponentsInChildren<IAttack>();
            if (_weapons != null && _weapons.Count > 0)
            {
                _primaryAttack = _weapons[0];
                _secondaryAttack = _weapons.Count > 1 ? _weapons[1] : _weapons[0];
            }
        }

        public void CancelAttack()
        {
            _primaryAttack.CancelAttack();
            _secondaryAttack.CancelAttack();
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

        public bool FirePrimary()
        {
            if (_primaryAttack == null)
            {
                Debug.Log("No primary weapon selected");
                return false;
            }

            return FireAttack(_primaryAttack);
        }

        private bool FireAttack(IAttack attack)
        {
            if (Stats == null && attack.RequiredEnergy > 0)
            {
                Debug.Log("Weapon requires energy to use, but no energy component attached.");
                return false;
            }

            if (Stats != null && Stats.IsStatDefined(StatsEnum.Energy) && !Stats.HasEnough(StatsEnum.Energy, attack.RequiredEnergy))
            {
                Debug.Log("Not enaugh energy.");
                return false;
            }

            if (!attack.CanFire) return false;

            attack.Fire();
            if (Stats != null && Stats.IsStatDefined(StatsEnum.Energy))
            {
                Stats.AddAmount(StatsEnum.Energy, -attack.RequiredEnergy);
            }

            return true;
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
