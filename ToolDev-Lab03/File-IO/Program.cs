using System;
using System.IO;
using System.Linq;
using System.Text;

namespace File_IO
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\TestFile.txt";

            if (!File.Exists(fileName))
            {
                string[] heroes = { "Squirrel Girl", "Batman", "Orgasmo", "Mysterio", "Gypsy Danger" };
                File.WriteAllLines(fileName, heroes);
            }

            File.Copy(fileName, AppDomain.CurrentDomain.BaseDirectory + "\\TestFileCopy.txt");

            File.Delete(fileName);
        }
    }
}