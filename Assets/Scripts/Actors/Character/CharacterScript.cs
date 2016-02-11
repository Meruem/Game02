using Assets.Scripts;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Assets.Scripts.Weapons;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public AmmoContainer AmmoContainer;
    public PlayerBasicGun Gun;

    public Transform WeaponArc;

    void Start()
    {
        if (AmmoContainer != null) AmmoContainer.AddAmmo(AmmoType.Bullets, 100);

        var UI = UIScript.Instance;
        if (UI != null)
        {
            UI.UpdateLives(_lives);

            if (AmmoContainer != null)
            {
                UI.UpdateAmmo(AmmoContainer.AmmoAmmount(AmmoType.Bullets));
            }
        }

        this.GetPubSub().SubscribeInContext<FireMessage>(m => Fire());
    }

    private void Fire()
    {
        if (Gun != null)
        {
            Gun.Fire(transform.position, WeaponArc.rotation.eulerAngles.z);
        }

        if (AmmoContainer != null)
        {
            UIScript.Instance.UpdateAmmo(AmmoContainer.AmmoAmmount(AmmoType.Bullets));
        }
    }
}
