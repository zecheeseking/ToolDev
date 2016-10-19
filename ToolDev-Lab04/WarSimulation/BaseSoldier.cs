namespace WarSimulation
{
    internal abstract class BaseSoldier
    {
        public delegate void OnSoldierHealed(BaseSoldier soldier, ICanHeal healer);

        public event OnSoldierHealed OnHealHandler;

        public delegate void OnSoldierHit(BaseSoldier soldier, IGun gun);

        public event OnSoldierHit OnHitHandler;

        public delegate void OnSoldierDied(BaseSoldier soldier, IGun gun);

        public event OnSoldierDied OnDiedHandler;


        public string Name { get; set; }
        public int Health { get; set; }
        public string TeamName { get; set; }
        public bool IsDead { get { return Health <= 0; } }

        protected BaseSoldier(string name, string teamName)
        {
            Name = name;
            TeamName = teamName;
            Health = 30;
        }

        public void Heal(ICanHeal healer)
        {
            Health += healer.HealAmount;
            OnHealHandler(this, healer);
        }

        public void Hit(IGun gun)
        {
            Health -= gun.DamagePerShot;

            if (IsDead)
                OnDiedHandler(this, gun);
            else
                OnHitHandler(this, gun);
        }
    }
}