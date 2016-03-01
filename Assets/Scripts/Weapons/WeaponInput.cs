using UnityEngine;

namespace Assets.Scripts.Weapons
{
    [RequireComponent(typeof (WeaponManager))]
    public class WeaponInput : MonoBehaviour
    {
        private readonly KeyCode[] _keyCodes =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };

        private WeaponManager _weaponManager;

        public void Awake()
        {
            _weaponManager = GetComponent<WeaponManager>();
        }

        public void Update()
        {
            for (int i = 0; i < _keyCodes.Length; i++)
            {
                if (Input.GetKeyDown(_keyCodes[i]))
                {
                    var numberPressed = i;
                    if (Input.GetKeyDown(KeyCode.LeftAlt))
                    {
                        _weaponManager.ChangeSecondaryWeapon(numberPressed);
                    }
                    else
                    {
                        _weaponManager.ChangePrimaryWeapon(numberPressed);
                    }
                }

            }
        }
    }
}