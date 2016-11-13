using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Data;
using System.Xml.Serialization;

namespace RecapLab.Viewmodel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ICollectionView _customersView { get; set; }
        private ObservableCollection<Customer> _customers;

        public ObservableCollection<Customer> Customers
        {
            get
            {
                if (_customers == null)
                {
                    _customers = new ObservableCollection<Customer>();

                    this._customersView = CollectionViewSource.GetDefaultView(_customers);
                }
                return _customers;
            }
            set { _customers = value; }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer { get { return _selectedCustomer; } set { _selectedCustomer = value; NotifyPropertyChanged("SelectedCustomer"); } }

        public string FilterParam
        {
            get { return _filterParam; }
            set
            {
                _filterParam = value;

                if (string.IsNullOrEmpty(_filterParam))
                    this._customersView.Filter = null;
                else
                    this._customersView.Filter = FilterByCountry;
            }
        }

        private string _filterParam;

        public MainWindowViewModel()
        {
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

        public void DeleteCustomer()
        {
            if (SelectedCustomer != null)
            {
                Customers.Remove(SelectedCustomer);
            }
        }

        private bool FilterByCountry(object cust)
        {
            Customer c = cust as Customer;

            if (c.Country == _filterParam)
                return true;

            return false;
        }

        public void SaveCustomerList()
        {
            SaveToXml<ObservableCollection<Customer>>(MainWindow.m_Path + "customerList.xml", _customers);
        }

        public void SaveFilteredCustomerList()
        {
            List<Customer> filteredItems = new List<Customer>();
            foreach (Customer c in _customersView)
                filteredItems.Add(c);

            SaveToXml<List<Customer>>(MainWindow.m_Path + "customerListFiltered.xml", filteredItems);
        }

        private void SaveToXml<T>(string path, T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamWriter sr = new StreamWriter(path))
            {
                serializer.Serialize(sr, obj);
            }
        }

        public void RefreshData()
        {
            _customers.Clear();

            string[] files = Directory.GetFiles(MainWindow.m_Path);

            foreach (string f in files)
            {
                if (f.Contains(MainWindow.m_Extension))
                {
                    using (var stream = File.OpenRead(f))
                    {
                        var formatter = new BinaryFormatter();
                        var c = (Customer)formatter.Deserialize(stream);

                        _customers.Add(c);
                    }
                }
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