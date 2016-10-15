using System;
using System.IO;
using System.Linq;
using System.Text;

namespace FileInfo_IO
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\TestFile.txt";

            FileInfo myFile = new FileInfo(fileName);

            if (!myFile.Exists)
            {
                using (StreamWriter sw = myFile.CreateText())
                {
                    sw.WriteLine("Squirrel Girl");
                    sw.WriteLine("Batman");
                    sw.WriteLine("Orgasmo");
                    sw.WriteLine("Mysterio");
                    sw.WriteLine("Gypsy Danger");
                }
            }

            string fileName2 = AppDomain.CurrentDomain.BaseDirectory + "\\TestFile2.txt";
            FileInfo myFile2 = new FileInfo(fileName2);

            if (File.Exists(fileName2))
                myFile2.Delete();

            myFile.CopyTo(fileName2);

            if (File.Exists(fileName))
                myFile.Delete();
        }
    }
}