using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Path_Ex
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string root = "d:\\Games";

            DirectoryInfo myDir = new DirectoryInfo(root);

            if (!myDir.Exists)
                myDir.Create();

            Console.WriteLine(Path.IsPathRooted(root));
            Console.WriteLine(Path.GetExtension(root + "steam.dll"));
            Console.ReadLine();
        }
    }
}