using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public void Start()
    {
        this.GetPubSub().SubscribeInContext<WeaponHitMessage>(m => HandleWeaponHit((WeaponHitMessage)m));
    }

    private void HandleWeaponHit(WeaponHitMessage weaponHitMessage)
    {
        this.GetPubSub().PublishMessageInContext(new TakeDamageMessage(weaponHitMessage.Damage));
    }
}
