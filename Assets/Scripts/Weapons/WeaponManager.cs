using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        private IList<IWeapon> _weapons = new List<IWeapon>();
        private IWeapon _primaryWeapon;
        private IWeapon _secondaryWeapon;

        public void Awake()
        {
            _weapons = GetComponentsInChildren<IWeapon>();
            if (_weapons != null && _weapons.Count > 0)
            {
                _primaryWeapon = _weapons[0];
                _secondaryWeapon = _weapons.Count > 1 ? _weapons[1] : _weapons[0];
            }
        }

        public void ChangePrimaryWeapon(int weaponNumber)
        {
            if (!CheckWeaponCount(weaponNumber)) return;
            _primaryWeapon = _weapons[weaponNumber];
        }

        public void ChangeSecondaryWeapon(int weaponNumber)
        {
            if (!CheckWeaponCount(weaponNumber)) return;
            _secondaryWeapon = _weapons[weaponNumber];
        }

        public void FirePrimary()
        {
            if (_primaryWeapon == null)
            {
                Debug.Log("No primary weapon selected");
                return;
            }

            _primaryWeapon.Fire();
        }

        public void FireSecondary()
        {
            if (_secondaryWeapon == null)
            {
                Debug.Log("No secondary weapon selected");
                return;
            }

            _secondaryWeapon.Fire();
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
