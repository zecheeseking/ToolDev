using RecapLab.Viewmodel;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace RecapLab
{
    /// <summary>
    /// Interaction logic for NewCustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        private Customer _customer;

        public CustomerWindow(Customer cust = null)
        {
            InitializeComponent();


            if (cust == null)
            {
                _customer = new Customer(GenerateId());
                TxtB_ID.Text = _customer.Id.ToString();
            }
            else
            {
                _customer = cust;
                TxtB_ID.Text = _customer.Id.ToString();
                Txtbox_Name.Text = _customer.Surname;
                Txtbox_FirstName.Text = _customer.Firstname;
                Txtbox_Street.Text = _customer.Street;
                Txtbox_City.Text = _customer.City;
                Txtbox_Country.Text = _customer.Country;
                Txtbox_Phone.Text = _customer.Phone;
            }
        }

        private int GenerateId()
        {
            int tmp = 0;

            Random rand = new Random();

            for (int i = 0; i < 9; ++i)
            {
                int r = rand.Next(10);
                tmp *= 10;
                tmp += r;
            }

            return tmp;
        }

        private void btn_NewCustomerOkClick(object sender, RoutedEventArgs e)
        {
            _customer.Surname = Txtbox_Name.Text;
            _customer.Firstname = Txtbox_FirstName.Text;
            _customer.Street = Txtbox_Street.Text;
            _customer.City = Txtbox_City.Text;
            _customer.Country = Txtbox_Country.Text;
            _customer.Phone = Txtbox_Phone.Text;

            MainWindowViewModel dataContext = Application.Current.MainWindow.DataContext as MainWindowViewModel;
            dataContext.AddCustomer(_customer);

            string path = MainWindow.m_Path + _customer.Id + MainWindow.m_Extension;

            if (File.Exists(path))
                File.Delete(path);

            using (var stream = File.Create(path))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, _customer);
            }

            this.Close();
        }
    }
}