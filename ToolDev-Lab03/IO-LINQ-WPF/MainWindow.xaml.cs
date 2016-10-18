using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace IO_LINQ_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Book> books = new List<Book>();

        private IEnumerable<Book> QueryGenre;


        public MainWindow()
        {
            InitializeComponent();

            QueryGenre =
                from b in books
                select b;
        }

        private void LoadFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (*.xml)|*xml";
            dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string fileName = dlg.FileName;
                XElement xml = XElement.Load(fileName);
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
            }

            Listbox_Display.Items.Clear();

            foreach (var b in books)
            {
                Listbox_Display.Items.Add(b);
            }
        }

        private void SortAscending(object sender, RoutedEventArgs e)
        {
            books.Sort(delegate (Book a, Book b)
            {
                return a.Title.CompareTo(b.Title);
            });
        }

        private void SortDescending(object sender, RoutedEventArgs e)
        {
            books.Sort(delegate (Book a, Book b)
            {
                return b.Title.CompareTo(a.Title);
            });
        }

        private void Btn_OrderClick(object sender, RoutedEventArgs e)
        {
            Listbox_Display.Items.Clear();

            foreach (Book b in books)
                Listbox_Display.Items.Add(b);
        }

        private void Btn_ShowDescClick(object sender, RoutedEventArgs e)
        {
            Book b = Listbox_Display.SelectedItem as Book;

            if (b != null)
                MessageBox.Show(b.Desc, b.Title, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
        }

        private void BtnSetQuery(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)ComboBox_Genre.SelectedItem;
            string genre = item.Content.ToString();

            if (genre == "All")
                QueryGenre =
                    from b in books
                    select b;
            else
                QueryGenre =
                    from b in books
                    where b.Genre == genre
                    select b;
        }

        private void BtnFilterListbox(object sender, RoutedEventArgs e)
        {
            Listbox_Display.Items.Clear();

            foreach (var i in QueryGenre)
            {
                Listbox_Display.Items.Add(i);
            }
        }
    }
}