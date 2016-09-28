using System;
using System.Text;

namespace EX7
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Soldiercs s = new Soldiercs("Steve");
            Console.WriteLine(s.ToString());
            s.ShootAt(s);
            Console.WriteLine(s.ToString());

            Console.ReadLine();
        }
    }
}