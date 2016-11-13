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

        private void Btn_ShowNewCustomerWindow(object sender, RoutedEventArgs e)
        {
            var window = new CustomerWindow();
            window.Show();
        }

        private void Btn_DeleteCustomerClick(object sender, RoutedEventArgs e)
        {
            viewModel.DeleteCustomer();
        }

        private void Btn_FilterClick(object sender, RoutedEventArgs e)
        {
            viewModel.FilterParam = Txtbox_FilterParams.Text;
        }

        private void Listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Listbox_Cust.SelectedIndex > -1)
            {
                Btn_EditCustomer.IsEnabled = true;
                Btn_DeleteCustomer.IsEnabled = true;
                Btn_SaveToXml.IsEnabled = true;
            }
        }

        private void Btn_ShowEditCustomerWindow(object sender, RoutedEventArgs e)
        {
            var window = new CustomerWindow(viewModel.SelectedCustomer);
            window.Show();
        }

        private void Btn_SaveToXmlClick(object sender, RoutedEventArgs e)
        {
            viewModel.SaveCustomerList();
        }

        private void Btn_SaveFilteredCustomersClick(object sender, RoutedEventArgs e)
        {
            viewModel.SaveFilteredCustomerList();
        }

        private void Btn_ReloadDataClick(object sender, RoutedEventArgs e)
        {
            viewModel.RefreshData();
        }
    }
}