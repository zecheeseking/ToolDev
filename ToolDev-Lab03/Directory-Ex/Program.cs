using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Directory_Ex
{
    internal class Program
    {
        private static int PrintRecursiveDirectory(string dir)
        {
            string[] files = Directory.GetFiles(dir);

            foreach (string s in files)
                Console.WriteLine(s);

            string[] directories = Directory.GetDirectories(dir);

            if (directories.Length == 0)
                return 1;
            else
            {
                foreach (string s in directories)
                    PrintRecursiveDirectory(s);
            }

            return 0;
        }

        private static void Main(string[] args)
        {
            string root = "d:\\Games";
            if (Directory.Exists(root))
            {
                PrintRecursiveDirectory(root);
            }
        }
    }
}