using Assets.Scripts.Actors.Stats;

namespace Assets.Scripts.Weapons
{
    public interface IAttack
    {
        void Fire(Stats stats);
        int Id { get; }
        int RequiredEnergy { get; }
        bool CanFire(Stats stats);
        void CancelAttack();
    }
}
