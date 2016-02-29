using System.Collections.Generic;
using Assets.Scripts.Messages;
using Assets.Scripts.Messages.Input;
using Assets.Scripts.Misc;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public float CharacterSpeed = 5;
    public float CharacterShieldedSpeed = 3;

    private bool _isShielded;
    private bool _isAttacking;

    private readonly List<int> _blockedWeaponIds = new List<int>();
    private readonly List<WeaponHitMessage> _unresolvedMessage = new List<WeaponHitMessage>(); 
    private float _nextReset;
    private float _timeDeltaWait = 0.05f;

    public void Start()
    {
        // Take damage when hit by weapon
        this.GetPubSub().SubscribeInContext<WeaponHitMessage>(m => HandleWeaponHit((WeaponHitMessage)m));

        this.GetPubSub().SubscribeInContext<MoveInputMessage>(m => HandleMoveInput((MoveInputMessage)m));

        this.GetPubSub().SubscribeInContext<FireInputMessage>(m => HandleFireInput((FireInputMessage)m));

        this.GetPubSub().SubscribeInContext<ShieldInputMessage>(m => HandleShieldInput((ShieldInputMessage)m));

        this.GetPubSub().SubscribeInContext<AttackEndedMessage>(m => HandleAttackEnded());

        this.GetPubSub().SubscribeInContext<ShieldHitMessage>(m => HandleShieldHitMessage((ShieldHitMessage)m));
    }

    public void Update()
    {
        if (_nextReset > 0 && Time.time > _nextReset)
        {
            _blockedWeaponIds.Clear();
            foreach (var message in _unresolvedMessage)
            {
                this.GetPubSub().PublishMessageInContext(new TakeDamageMessage(message.Damage));
            }
            _unresolvedMessage.Clear();
            _nextReset = 0;
        }
    }

    private void HandleShieldHitMessage(ShieldHitMessage shieldHitMessage)
    {
        _blockedWeaponIds.Add(shieldHitMessage.Weapon.GetInstanceID());
        _nextReset = Time.time + _timeDeltaWait;
    }

    private void HandleAttackEnded()
    {
        _isAttacking = false;
        this.GetPubSub().PublishMessageInContext(new ShieldChangeMessage(_isShielded));
    }

    private void HandleShieldInput(ShieldInputMessage shieldInputMessage)
    {
        if (_isShielded != shieldInputMessage.ShieldUp)
        {
            _isShielded = shieldInputMessage.ShieldUp;
            if (!_isAttacking)
            {
                this.GetPubSub().PublishMessageInContext(new ShieldChangeMessage(_isShielded));
            }
        }
    }

    private void HandleFireInput(FireInputMessage fireInputMessage)
    {
        _isAttacking = true;
        if (_isShielded)
        {
            this.GetPubSub().PublishMessageInContext(new ShieldChangeMessage(false));
        }

        this.GetPubSub().PublishMessageInContext(new FireMessage(fireInputMessage.IsSecondary));
    }

    private void HandleMoveInput(MoveInputMessage moveInputMessage)
    {
        this.GetPubSub().PublishMessageInContext(new MoveInDirectionMessage(moveInputMessage.Direction, false, _isShielded ? CharacterShieldedSpeed : CharacterSpeed));
    }

    private void HandleWeaponHit(WeaponHitMessage weaponHitMessage)
    {
        if (_blockedWeaponIds.Contains(weaponHitMessage.Weapon.GetInstanceID())) return;
        _unresolvedMessage.Add(weaponHitMessage);
        _nextReset = Time.time + _timeDeltaWait;
    }
}
