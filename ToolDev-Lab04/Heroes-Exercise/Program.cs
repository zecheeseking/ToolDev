using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes_Exercise
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<BaseHero> heroes = new List<BaseHero>();

            heroes.Add(new Robin());
            heroes.Add(new Batman());
            heroes.Add(new Superman());

            Console.WriteLine("List of hero's: ");
            foreach (BaseHero hero in heroes)
                Console.WriteLine("\t" + hero.SayName());

            Console.WriteLine("Hero's that can fly: ");
            foreach (BaseHero hero in heroes)
                if (hero is ICanFly)
                {
                    ICanFly h = hero as ICanFly;
                    Console.WriteLine("\t" + h.Fly());
                }

            Console.WriteLine("Hero's that can jump: ");
            foreach (BaseHero hero in heroes)
                if (hero is ICanJump)
                {
                    ICanJump h = hero as ICanJump;
                    Console.WriteLine("\t" + h.Jump());
                }

            Console.WriteLine("Hero's that can swim: ");
            foreach (BaseHero hero in heroes)
                if (hero is ICanSwim)
                {
                    ICanSwim h = hero as ICanSwim;
                    Console.WriteLine("\t" + h.Swim());
                }

            Console.WriteLine("Hero's that have X-Ray Vision: ");
            foreach (BaseHero hero in heroes)
                if (hero is IHasXRayVision)
                {
                    IHasXRayVision h = hero as IHasXRayVision;
                    Console.WriteLine("\t" + h.SeeThroughStuff());
                }

            Console.WriteLine("Hero's that are useless: ");
            foreach (BaseHero hero in heroes)
                if (hero is IUseless)
                {
                    IUseless h = hero as IUseless;
                    Console.WriteLine("\t" + h.Die());
                }

            Console.ReadLine();
        }
    }
}