using System;
using System.Text;

namespace EX7
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Soldier s = new Soldier("Steve");
            Console.WriteLine(s.ToString());
            s.ShootAt(s);
            Console.WriteLine(s.ToString());

            Console.ReadLine();
        }
    }
}