namespace WarSimulation
{
    internal class Medic : BaseSoldier, ICanHeal
    {
        public int HealAmount { get; set; }

        public Medic(string name, string teamName) : base(name, teamName)
        {
            HealAmount = 10;
        }
    }
}