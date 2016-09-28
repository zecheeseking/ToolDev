using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX6
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("First Name: ");
            string first = Console.ReadLine();
            Console.WriteLine("Last Name: ");
            string last = Console.ReadLine();
            Console.WriteLine("Birthday (no year): ");
            string date = Console.ReadLine();
            var splits = date.Split();
            int day = int.Parse(splits[0]);
            int month = int.Parse(splits[1]);

            DateTime today = DateTime.Today;
            DateTime birthday = new DateTime(today.Year, month, day);

            if (birthday < today)
                birthday.AddYears(1);

            int daysLeft = (birthday - today).Days;

            Console.WriteLine($"Hello {first} {last}, your birthday is in {daysLeft} days.");
            Console.ReadLine();
        }
    }
}