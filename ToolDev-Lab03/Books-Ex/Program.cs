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

            XElement xml = XElement.Load(path);

            List<Book> books = new List<Book>();

            XName book = XName.Get("book");

            foreach (var element in xml.Elements(book))
            {
                books.Add(new Book()
                {
                    Id = element.FirstAttribute.Value,
                    Author = element.Element(XName.Get("author")).Value,
                    Title = element.Element(XName.Get("title")).Value,
                    Genre = element.Element(XName.Get("genre")).Value,
                    Price = Convert.ToDouble(element.Element(XName.Get("price")).Value),
                    PublishedDate = Convert.ToDateTime(element.Element(XName.Get("publish_date")).Value),
                    Desc = element.Element(XName.Get("description")).Value
                });
            }

            foreach (Book b in books)
                Console.WriteLine(b.Print());

            Console.ReadLine();
        }
    }
}