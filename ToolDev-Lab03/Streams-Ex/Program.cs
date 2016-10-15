using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Streams_Ex
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\TestFile.txt";

            using (StreamWriter sr = new StreamWriter(path))
            {
                sr.WriteLine("The nightmare lord sings to us");
                sr.WriteLine("He joins us together in the lucid dream");
                sr.WriteLine("Good thing I've got insomnia");
                sr.WriteLine("Get bent Yogg'zoth");
            }

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    Console.WriteLine(line);
            }

            Console.ReadLine();
        }
    }
}