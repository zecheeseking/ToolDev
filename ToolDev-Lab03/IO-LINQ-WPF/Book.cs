using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO_LINQ_WPF
{
    internal class Book
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public double Price { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Desc { get; set; }

        public override string ToString()
        {
            return "(ID: " + Id + ") " + Title + " [" + Genre + "]";
        }
    }
}