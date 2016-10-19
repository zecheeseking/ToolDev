using System;
using System.Collections.Generic;
using System.Threading;

namespace WarSimulation
{
    internal class CommandCenter
    {
        private List<BaseSoldier> _teamA = new List<BaseSoldier>();
        private List<BaseSoldier> _teamB = new List<BaseSoldier>();
        private Random _rand = new Random();

        public bool WarOver { get; set; }

        public void SoldierHealedEvent(BaseSoldier soldier, ICanHeal healer)
        {
            Console.WriteLine("Soldier {0} of Team {1} was healed. +{2}", soldier.Name, soldier.TeamName, healer.HealAmount);
        }

        public void SoldierHitEvent(BaseSoldier soldier, IGun gun)
        {
            Console.WriteLine("Soldier {0} of Team {1} was hit by a {2}. (-{3})", soldier.Name, soldier.TeamName, gun.GunType, gun.DamagePerShot);
        }

        public void SoldierDiedEvent(BaseSoldier soldier, IGun gun)
        {
            Console.WriteLine("Soldier {0} of Team {1} was hit by a {2} and died!", soldier.Name, soldier.TeamName, gun.GunType);

            _teamA.Remove(soldier);
            _teamB.Remove(soldier);
        }

        public void CreateTeams()
        {
            _teamA.Clear();
            _teamB.Clear();

            int armySize = 10;

            for (int i = 0; i < armySize; ++i)
            {
                Gunner g = new Gunner("" + i, "A");
                g.OnHitHandler += SoldierHitEvent;
                g.OnHealHandler += SoldierHealedEvent;
                g.OnDiedHandler += SoldierDiedEvent;

                _teamA.Add(g);

                g = new Gunner("" + i, "B");
                g.OnHitHandler += SoldierHitEvent;
                g.OnHealHandler += SoldierHealedEvent;
                g.OnDiedHandler += SoldierDiedEvent;

                _teamB.Add(g);
            }

            for (int i = 0; i < armySize / 5; ++i)
            {
                int index = _rand.Next(0, armySize);
                Medic m = new Medic("" + index, "A");
                m.OnHitHandler += SoldierHitEvent;
                m.OnHealHandler += SoldierHealedEvent;
                m.OnDiedHandler += SoldierDiedEvent;
                _teamA[index] = m;

                index = _rand.Next(0, armySize);
                m = new Medic("" + index, "B");
                m.OnHitHandler += SoldierHitEvent;
                m.OnHealHandler += SoldierHealedEvent;
                m.OnDiedHandler += SoldierDiedEvent;
                _teamB[index] = m;
            }
        }

        public void Fight()
        {
            if (_teamA.Count == 0)
            {
                WarOver = true;
                Console.WriteLine("War over! Team B won with {0} survivors!", _teamB.Count);
                return;
            }
            else if (_teamB.Count == 0)
            {
                WarOver = true;
                Console.WriteLine("War over! Team A won with {0} survivors!", _teamA.Count);
                return;
            }

            int indexA = _rand.Next(0, _teamA.Count);
            int indexB = _rand.Next(0, _teamB.Count);
            int side = _rand.Next(0, 100);
            BaseSoldier first, second;
            if (side < 50)
            {
                first = _teamA[indexA];
                second = _teamB[indexB];
            }
            else
            {
                first = _teamB[indexB];
                second = _teamA[indexA];
            }

            if (first is ICanHeal)
            {
                Console.WriteLine(">> Soldier {0} of Team {1} starts healing...", first.Name, first.TeamName);
                Thread.Sleep(500);
                ICanHeal medic = first as ICanHeal;
                first.Heal(medic);
            }
            else if (first is IGun)
            {
                Console.WriteLine(">> Soldier {0} of Team {1} starts shooting...", first.Name, first.TeamName);
                Thread.Sleep(500);
                IGun gunner = first as IGun;
                second.Hit(gunner);
            }
        }
    }
}