using System;
using System.Text;

namespace EX3
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sArr = Console.ReadLine().Split(' ');
            double sum = 0.0;

            foreach (string s in sArr)
                sum += double.Parse(s);

            Console.WriteLine($"sum of values: {sum}");
            Console.ReadLine();
        }
    }
}