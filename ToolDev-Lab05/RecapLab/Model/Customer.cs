using System;
using System.ComponentModel;

namespace RecapLab
{
    [Serializable]
    public class Customer : INotifyPropertyChanged
    {
        private int _id;
        private string _surname;
        private string _firstName;
        private string _street;
        private string _city;
        private string _country;
        private string _phone;

        public int Id { get { return _id; } }
        public string Surname { get { return _surname; } set { _surname = value; OnPropertyChanged("Surname"); } }
        public string Firstname { get { return _firstName; } set { _firstName = value; OnPropertyChanged("Firstname"); } }
        public string Street { get { return _street; } set { _street = value; OnPropertyChanged("Street"); } }
        public string City { get { return _city; } set { _city = value; OnPropertyChanged("City"); } }
        public string Country { get { return _country; } set { _country = value; OnPropertyChanged("Country"); } }
        public string Phone { get { return _phone; } set { _phone = value; OnPropertyChanged("Phone"); } }

        public Customer(int id)
        {
            _id = id;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", Firstname, Surname);
        }

        protected void OnPropertyChanged(string propertyName)
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