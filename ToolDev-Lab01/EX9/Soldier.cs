﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX9
{
    public class Soldier
    {
        private static Random rand = new Random();
        private int _health = 100;

        public string Name { get; private set; }
        public int Health { get { return _health; } set { } }
        public bool IsDead { get; private set; }

        public Soldier(string name)
        {
            Name = name;
        }

        public void Hit(int damage)
        {
            _health -= damage;

            if (_health <= 0)
                IsDead = true;
        }

        public void ShootAt(Soldier target)
        {
            target.Hit(rand.Next(0, 3));
        }

        public override string ToString()
        {
            return $"{Name} has {Health} health left.";
        }
    }
}