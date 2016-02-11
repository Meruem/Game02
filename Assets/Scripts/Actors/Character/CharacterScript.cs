using Assets.Scripts;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Assets.Scripts.Weapons;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public PlayerBasicGun Gun;

    public Transform WeaponArc;

    void Start()
    {
        this.GetPubSub().SubscribeInContext<FireMessage>(m => Fire());
    }

    private void Fire()
    {
        if (Gun != null)
        {
            Gun.Fire(transform.position, WeaponArc.rotation.eulerAngles.z);
        }
    }
}
