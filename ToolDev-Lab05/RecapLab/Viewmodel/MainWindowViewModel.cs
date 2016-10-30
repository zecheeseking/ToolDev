using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RecapLab.Viewmodel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers { get { return _customers; } set { _customers = value; } }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer { get { return _selectedCustomer; } set { _selectedCustomer = value; NotifyPropertyChanged("SelectedCustomer"); } }

        public MainWindowViewModel()
        {
            _customers = new ObservableCollection<Customer>();
        }

        public void AddCustomer(Customer cust)
        {
            for (int i = 0; i < _customers.Count; ++i)
            {
                if (_customers[i].Id == cust.Id)
                {
                    _customers[i] = cust;
                    return;
                }
            }

            _customers.Add(cust);
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, args);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}