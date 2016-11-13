using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WarSimulation
{
    internal class CommandCenter : INotifyPropertyChanged
    {
        private List<BaseSoldier> _teamA = new List<BaseSoldier>();
        private List<BaseSoldier> _teamB = new List<BaseSoldier>();
        private Random _rand = new Random();

        private ObservableCollection<string> _warLog;
        public ObservableCollection<string> WarLog
        {
            get
            {
                if(_warLog == null)
                {
                    _warLog = new ObservableCollection<string>();
                }

                return _warLog;
            }
            private set
            {
                _warLog = value;
                OnPropertyChanged("WarLog");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Add(string s)
        {
            WarLog.Add(s);
            OnPropertyChanged("WarLog");
        }

        private bool _warOver = true;
        public bool WarOver { get { return _warOver; } private set { _warOver = value;  OnPropertyChanged("WarOver"); } }

        public void SoldierHealedEvent(BaseSoldier soldier, ICanHeal healer)
        {
            Add("Soldier " + soldier.Name + " of Team " + soldier.TeamName + " was healed. + " + healer.HealAmount);
        }

        public void SoldierHitEvent(BaseSoldier soldier, IGun gun)
        {
            Add("Soldier " + soldier.Name + " of Team " + soldier.TeamName + " was hit by a " + gun.GunType + ". (-" + gun.DamagePerShot + ")");
        }

        public void SoldierDiedEvent(BaseSoldier soldier, IGun gun)
        {
            Add("Soldier " + soldier.Name + " of Team " + soldier.TeamName + " was hit by a " + gun.GunType + " and died!");

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

            WarOver = false;
        }

        public void Fight()
        {
            if (_teamA.Count == 0)
            {
                WarOver = true;
                Add("War over! Team B won with " + _teamB.Count + " survivors!");
                return;
            }
            else if (_teamB.Count == 0)
            {
                WarOver = true;
                Add("War over! Team A won with " + _teamA.Count + " survivors!");
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
                Add(">> Soldier " + first.Name + " of Team first.TeamName starts healing...");
                Thread.Sleep(500);
                ICanHeal medic = first as ICanHeal;
                first.Heal(medic);
            }
            else if (first is IGun)
            {
                Add(">> Soldier " + first.Name + " of Team " + first.TeamName + " starts shooting...");
                Thread.Sleep(500);
                IGun gunner = first as IGun;
                second.Hit(gunner);
            }
        }
    }
}