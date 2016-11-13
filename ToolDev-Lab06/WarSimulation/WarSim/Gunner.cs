namespace WarSimulation
{
    internal class Gunner : BaseSoldier, IGun
    {
        public string GunType { get; set; }
        public int DamagePerShot { get; set; }

        public Gunner(string name, string teamName) : base(name, teamName)
        {
            GunType = "Pistol";
            DamagePerShot = 10;
        }
    }
}