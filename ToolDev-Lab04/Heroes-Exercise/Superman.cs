using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes_Exercise
{
    internal class Superman : BaseHero, ICanFly, ICanJump, ICanSwim, IHasXRayVision
    {
        public Superman()
        {
            Name = "Superman";
        }


        public string Jump()
        {
            return Name + " jumped to space";
        }

        public string Swim()
        {
            return Name + " swam to Lois Lane";
        }

        public string Fly()
        {
            return Name + " flew to Paris";
        }

        public string SeeThroughStuff()
        {
            return Name + " saw your underpants";
        }
    }
}