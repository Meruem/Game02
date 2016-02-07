using Assets.Scripts;
using Assets.Scripts.Weapons;
using UnityEngine;

[RequireComponent(typeof(MoveScript))]
public class CharacterScript : MonoBehaviour
{
    private bool _mousePressed;
    private MoveScript _moveScript;
    private Transform _weaponArc;
    private AmmoContainer _ammoContainer;
    private WeaponBase _activeWeapon;

    private int _lives = 10;

    void Start()
    {
        _moveScript = GetComponent<MoveScript>();
        _weaponArc = transform.FindChild("Weapon");
        _ammoContainer = new AmmoContainer();
        _ammoContainer.AddAmmo(AmmoType.Bullets, 100);
        _activeWeapon = new PlayerBasicGun(_ammoContainer, 0.2f);
        var damageTrigger = GetComponent<TakeDamageTrigger>();
        damageTrigger.OnTakeDamage += TakeDamage;

        var UI = UIScript.Instance;
        if (UI != null)
        {
            UI.UpdateLives(_lives);
            UI.UpdateAmmo(_ammoContainer.AmmoAmmount(AmmoType.Bullets));
        }
    }

    private void TakeDamage(int damage)
    {
        _lives -= damage;
        UIScript.Instance.UpdateLives(_lives);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mousePressed = true;
        }
    }
 
    public void FixedUpdate()
    {
        float xMovement = Input.GetAxisRaw("Horizontal");
        float yMovement = Input.GetAxisRaw("Vertical");

        var movement = new Vector2(xMovement, yMovement);

        if (_mousePressed)
        {
            Fire();
            _mousePressed = false;
        }

        _moveScript.Move(movement.normalized);
    }

    private void Fire()
    {
        _activeWeapon.Fire(transform.position, _weaponArc.rotation.eulerAngles.z);
        UIScript.Instance.UpdateAmmo(_ammoContainer.AmmoAmmount(AmmoType.Bullets));
    }
}