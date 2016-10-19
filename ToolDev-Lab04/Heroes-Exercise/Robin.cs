using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes_Exercise
{
    internal class Robin : BaseHero, IUseless
    {
        public Robin()
        {
            Name = "Robin";
        }

        public string Die()
        {
            return Name + " died";
        }
    }
}