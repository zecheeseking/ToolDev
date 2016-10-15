using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Books_Ex
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Books.xml";

            var xml = XElement.Load(path);

            List<Book> books = new List<Book>();
        }
    }
}