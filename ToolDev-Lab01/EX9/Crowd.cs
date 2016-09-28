using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX9
{
    public class Crowd
    {
        private Soldier[] _crowd;

        public Crowd()
        {
            _crowd = new Soldier[10];

            for (int i = 0; i < 10; ++i)
                _crowd[i] = new Soldier($"Soldier {i}");
        }

        public void Fight()
        {
        }
    }
}