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
        private CustomerDetailsViewModel viewModel;

        public CustomerWindow(Customer cust = null)
        {
            InitializeComponent();
            viewModel = new CustomerDetailsViewModel();
            this.DataContext = viewModel;

            if (cust == null)
            {
                viewModel.Customer = new Customer();
            }
            else
                viewModel.Customer = cust;
        }

        private void btn_NewCustomerOkClick(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel vm = Application.Current.MainWindow.DataContext as MainWindowViewModel;
            string path = MainWindow.m_Path + viewModel.Customer.Id + MainWindow.m_Extension;
            vm.AddCustomer(viewModel.Customer);

            if (File.Exists(path))
                File.Delete(path);

            using (var stream = File.Create(path))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, viewModel.Customer);
            }

            this.Close();
        }
    }
}