using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Actors.Stats;
using Assets.Scripts.Messages;
using Assets.Scripts.Messages.Input;
using Assets.Scripts.Misc;
using Assets.Scripts.Weapons;
using UnityEngine;

[RequireComponent(typeof(IMoveScript))]
public class CharacterController : MonoBehaviour
{
    public float CharacterSpeed = 5;
    public float CharacterShieldedSpeed = 3;
    
    public WeaponManager WeaponManager;
    public Stats Stats;

    private bool _isShielded;
    private bool _isAttacking;

    private readonly List<int> _blockedWeaponIds = new List<int>();
    private readonly List<WeaponHitMessage> _unresolvedHitMessages = new List<WeaponHitMessage>(); 
    private float _nextReset;
    private float _timeDeltaWait = 0.01f;

    private IMoveScript _moveScript;

    public void Start()
    {
        _moveScript = GetComponent<IMoveScript>();

        // Take damage when hit by weapon
        this.GetPubSub().SubscribeInContext<WeaponHitMessage>(m => HandleWeaponHit((WeaponHitMessage)m));

        // Input handlers
        this.GetPubSub().SubscribeInContext<MoveInputMessage>(m => HandleMoveInput((MoveInputMessage)m));
        this.GetPubSub().SubscribeInContext<FireInputMessage>(m => HandleFireInput((FireInputMessage)m));
        this.GetPubSub().SubscribeInContext<ShieldInputMessage>(m => HandleShieldInput((ShieldInputMessage)m));

        this.GetPubSub().SubscribeInContext<AttackEndedMessage>(m => HandleAttackEnded());

        // On shield hit
        this.GetPubSub().SubscribeInContext<ShieldHitMessage>(m => HandleShieldHitMessage((ShieldHitMessage)m));

        Stats.AddRegen(StatsEnum.Energy, 10);
    }

    public void Update()
    {
        ApplyDamage();
    }

    private void ApplyDamage()
    {
        if (_nextReset > 0 && Time.time > _nextReset)
        {
            _blockedWeaponIds.Clear();
            foreach (var message in _unresolvedHitMessages)
            {
                Stats.AddAmount(StatsEnum.Health, -message.Damage);
            }
            _unresolvedHitMessages.Clear();
            _nextReset = 0;
        }
    }

    private void HandleShieldHitMessage(ShieldHitMessage shieldHitMessage)
    {
        _blockedWeaponIds.Add(shieldHitMessage.Weapon.GetInstanceID());
        _unresolvedHitMessages.RemoveAll(m => m.Weapon.GetInstanceID() == shieldHitMessage.Weapon.GetInstanceID());
        _nextReset = Time.time + _timeDeltaWait;
        Stats.AddAmount(StatsEnum.Energy, -shieldHitMessage.EnergyDamage);
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

        if (fireInputMessage.IsSecondary)
        {
            WeaponManager.FireSecondary();
        }
        else
        {
            WeaponManager.FirePrimary();
        }
    }

    private void HandleMoveInput(MoveInputMessage moveInputMessage)
    {
        _moveScript.MoveNormal(moveInputMessage.Direction, _isShielded ? CharacterShieldedSpeed : CharacterSpeed);
    }

    private void HandleWeaponHit(WeaponHitMessage weaponHitMessage)
    {
        if (_blockedWeaponIds.Contains(weaponHitMessage.Weapon.GetInstanceID())) return;
        _unresolvedHitMessages.Add(weaponHitMessage);
        _nextReset = Time.time + _timeDeltaWait;
    }
}
