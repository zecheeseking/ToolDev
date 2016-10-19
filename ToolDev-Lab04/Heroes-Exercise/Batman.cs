using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes_Exercise
{
    internal class Batman : BaseHero, ICanJump, ICanSwim
    {
        public Batman()
        {
            Name = "Batman";
        }

        public string Jump()
        {
            return Name + " jumped high";
        }

        public string Swim()
        {
            return Name + " can swim to Gotham";
        }
    }
}