using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarSimulation
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CommandCenter cc = new CommandCenter();

            cc.CreateTeams();

            while (!cc.WarOver)
            {
                Console.ReadLine();
                cc.Fight();
                Console.WriteLine(">>");
            }


            Console.ReadLine();
        }
    }
}