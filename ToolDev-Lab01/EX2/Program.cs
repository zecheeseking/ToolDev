using System;
using System.Text;

namespace EX2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Write("First name: ");
            string first = Console.ReadLine();
            Console.Write("Last name: ");
            string last = Console.ReadLine();
            Console.WriteLine($"Greetings {first} {last}");
            Console.ReadLine();
        }
    }
}