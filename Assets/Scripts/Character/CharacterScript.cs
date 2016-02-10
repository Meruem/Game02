using Assets.Scripts;
using Assets.Scripts.Weapons;
using UnityEngine;

[RequireComponent(typeof(MoveScript))]
public class CharacterScript : MonoBehaviour
{
    public AmmoContainer AmmoContainer;
    public PlayerBasicGun Gun;
    //private WeaponBase _activeWeapon;

    private bool _mousePressed;
    private MoveScript _moveScript;
    private Transform _weaponArc;

    private int _lives = 10;

    void Start()
    {
        _moveScript = GetComponent<MoveScript>();
        _weaponArc = transform.FindChild("Weapon");
        //AmmoContainer = new AmmoContainer();
        if (AmmoContainer != null) AmmoContainer.AddAmmo(AmmoType.Bullets, 100);
        var damageTrigger = GetComponent<TakeDamageTrigger>();
        damageTrigger.OnTakeDamage += TakeDamage;

        var UI = UIScript.Instance;
        if (UI != null)
        {
            UI.UpdateLives(_lives);

            if (AmmoContainer != null)
            {
                UI.UpdateAmmo(AmmoContainer.AmmoAmmount(AmmoType.Bullets));
            }
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
        if (Gun != null)
        {
            Gun.Fire(transform.position, _weaponArc.rotation.eulerAngles.z);
        }

        if (AmmoContainer != null)
        {
            UIScript.Instance.UpdateAmmo(AmmoContainer.AmmoAmmount(AmmoType.Bullets));
        }
    }
}