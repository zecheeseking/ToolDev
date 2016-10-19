namespace WarSimulation
{
    public interface ICanHeal
    {
        int HealAmount { get; set; }
    }

    public interface IGun
    {
        string GunType { get; set; }

        int DamagePerShot { get; set; }
    }
}