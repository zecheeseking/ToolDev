using System;
using System.ComponentModel;

namespace RecapLab.Viewmodel
{
    internal class CustomerDetailsViewModel : INotifyPropertyChanged
    {
        private Customer _customer;

        public Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                NotifyPropertyChanged("Customer");
            }
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