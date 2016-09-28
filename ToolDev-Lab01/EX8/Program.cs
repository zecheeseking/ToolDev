using System;
using System.Text;

namespace EX8
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Soldier s1 = new Soldier("Teji");
            Soldier s2 = new Soldier("Evil Teji");

            Console.WriteLine($"{s1.Name} vs. {s2.Name}");
            Console.WriteLine();
            Random rand = new Random();

            while (!s1.IsDead && !s2.IsDead)
            {
                Console.ReadLine();

                int temp = rand.Next(0, 99);

                if (temp < 50)
                {
                    s1.ShootAt(s2);
                    s2.ShootAt(s1);
                    Console.WriteLine(s1.ToString());
                    Console.WriteLine(s2.ToString());
                    Console.WriteLine();
                }
                else
                {
                    s2.ShootAt(s1);
                    s1.ShootAt(s2);
                    Console.WriteLine(s2.ToString());
                    Console.WriteLine(s1.ToString());
                    Console.WriteLine();
                }

                if (s1.IsDead)
                    Console.WriteLine($"{s2.Name} wins!");
                else if (s2.IsDead)
                    Console.WriteLine($"{s1.Name} wins!");
            }

            Console.ReadLine();
        }
    }
}