namespace Assets.Scripts.Weapons
{
    public interface IAttack
    {
        void Fire();
        int Id { get; }
        int RequiredEnergy { get; }
    }
}
