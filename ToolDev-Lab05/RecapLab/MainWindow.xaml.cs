using RecapLab.Viewmodel;
using System.Windows;
using System.Windows.Controls;

namespace RecapLab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string m_Path = "data/";
        public static string m_Extension = ".cst";

        private MainWindowViewModel viewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }



        private void Btn_ShowerNewCustomerWindow(object sender, RoutedEventArgs e)
        {
            var window = new CustomerWindow();
            window.Show();
        }

        private void Btn_DeleteCustomerClick(object sender, RoutedEventArgs e)
        {
            //int id = _customers[Listbox_Cust.SelectedIndex].Id;
            //_customers.RemoveAt(Listbox_Cust.SelectedIndex);

            //File.Delete(m_Path + id.ToString() + m_Extension);

            //if (_customers.Count <= 0)
            //{
            //    Btn_EditCustomer.IsEnabled = false;
            //    Btn_DeleteCustomer.IsEnabled = false;
            //}
        }

        private void Btn_EditCustomerClicked(object sender, RoutedEventArgs e)
        {
            var window = new CustomerWindow((Customer)Listbox_Cust.SelectedItem);
            window.Show();
        }

        private void Btn_FilterClick(object sender, RoutedEventArgs e)
        {
            var newItems = Listbox_Cust.Items;




            Listbox_Cust.Items.Refresh();
        }
    }
}