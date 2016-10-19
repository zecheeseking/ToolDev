using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes_Exercise
{
    internal class BaseHero
    {
        private string _name;

        public string Name { get { return _name; } set { _name = value; } }

        public string SayName()
        {
            return "I'm " + _name;
        }
    }
}