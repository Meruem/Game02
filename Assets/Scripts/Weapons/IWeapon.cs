namespace Assets.Scripts.Weapons
{
    public interface IWeapon
    {
        void Fire();
        int Id { get; }
    }
}
