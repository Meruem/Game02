using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Actors.Stats;
using Assets.Scripts.Messages;
using Assets.Scripts.Messages.Input;
using Assets.Scripts.Misc;
using Assets.Scripts.Weapons;
using UnityEngine;
using Assets.Scripts.Actors.Character;
using UnityEngine.Assertions;

[RequireComponent(typeof (MoveScript))]
public class CharacterController : MonoBehaviour
{
    public float CharacterSpeed = 5;
    public float CharacterShieldedSpeed = 3;

    public int EnergyRegen = 10;
    public int StabilityRegen = 10;

    public WeaponManager WeaponManager;
    public Stats Stats;

    private bool _isShielded;
    private bool _isAttacking;
    private bool _isStaggered;

    private MoveScript _moveScript;
    private CharacterDamage _characterDamage;
    private Shield _shieldScript;

    public void Start()
    {
        _moveScript = GetComponent<MoveScript>();
        _shieldScript = GetComponentInChildren<Shield>();

        // Take damage when hit by weapon
        this.GetPubSub().SubscribeInContext<WeaponHitMessage>(m => HandleWeaponHit((WeaponHitMessage) m));

        // Input handlers
        this.GetPubSub().SubscribeInContext<MoveInputMessage>(m => HandleMoveInput((MoveInputMessage) m));
        this.GetPubSub().SubscribeInContext<FireInputMessage>(m => HandleFireInput((FireInputMessage) m));
        this.GetPubSub().SubscribeInContext<ShieldInputMessage>(m => HandleShieldInput((ShieldInputMessage) m));

        this.GetPubSub().SubscribeInContext<AttackEndedMessage>(m => HandleAttackEnded());

        // On shield hit
        this.GetPubSub().SubscribeInContext<ShieldHitMessage>(m => HandleShieldHitMessage((ShieldHitMessage) m));

        // On Stagger
        this.GetPubSub().SubscribeInContext<StaggeredMessage>(m => HandleStaggerMessage((StaggeredMessage)m));

        Assert.IsNotNull(Stats);

        Stats.AddRegen(StatsEnum.Energy, EnergyRegen);
        Stats.AddRegen(StatsEnum.Stability, StabilityRegen);

        _characterDamage = new CharacterDamage(Stats);
    }

    public void Update()
    {
        _characterDamage.ApplyDamage();
    }

    private void HandleStaggerMessage(StaggeredMessage message)
    {
        _isStaggered = true;

        // disable movement
        this.GetPubSub().PublishMessageInContext(new ForceMovementMessage(Vector2.zero, 0, message.Time, false));

        WeaponManager.CancelAttack();

        _isShielded = false;
        _shieldScript.ShieldDown();
        this.StartAfterTime(() => { _isStaggered = false; }, message.Time);
    }


    private void HandleShieldHitMessage(ShieldHitMessage shieldHitMessage)
    {
        if (!Stats.HasEnough(StatsEnum.Energy, shieldHitMessage.EnergyDamage))
        {
            Stats.AddAmount(StatsEnum.Health, -shieldHitMessage.OriginalDamage);
            _isShielded = false;
            _shieldScript.ShieldDown();
        }
        else
        {
            Stats.AddAmount(StatsEnum.Energy, -shieldHitMessage.EnergyDamage);
        }

        _characterDamage.RegisterNewBlockedDamageSource(shieldHitMessage.Weapon.GetInstanceID());
    }

    private void HandleAttackEnded()
    {
        _isAttacking = false;
        if (_isShielded) _shieldScript.ShieldUp();
        else _shieldScript.ShieldDown(); 
    }

    private void HandleShieldInput(ShieldInputMessage shieldInputMessage)
    {
        if (_isShielded != shieldInputMessage.ShieldUp)
        {
            _isShielded = shieldInputMessage.ShieldUp;
            if (!_isAttacking)
            {
                if (_isShielded) _shieldScript.ShieldUp();
                else _shieldScript.ShieldDown(); 
            }
        }
    }

    private void HandleFireInput(FireInputMessage fireInputMessage)
    {
        if (_isStaggered) return;

        _isAttacking = true;
        if (_isShielded)
        {
            _shieldScript.ShieldDown();
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
        Move(moveInputMessage.Direction);
    }

    private void Move(Vector2 direction)
    {
        if (_isStaggered) return;
        _moveScript.MoveNormal(direction, _isShielded ? CharacterShieldedSpeed : CharacterSpeed);
    }

    private void HandleWeaponHit(WeaponHitMessage weaponHitMessage)
    {
        _characterDamage.RegisterNewDamage(weaponHitMessage);
    }
}
