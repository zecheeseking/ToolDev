using System.Windows;
using GalaSoft.MvvmLight;
using System.Diagnostics;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Controls;

namespace ToolDev_IvyGenerator.CustomControls
{
    public partial class NumberTextbox : UserControl
    {
        public static DependencyProperty ValueProperty = DependencyProperty.Register(
        "Value", typeof(int), typeof(NumberTextbox), new FrameworkPropertyMetadata(0));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
        "MaxValue", typeof(int), typeof(NumberTextbox), new FrameworkPropertyMetadata(10));

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
        "MinValue", typeof(int), typeof(NumberTextbox), new FrameworkPropertyMetadata(0));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        private RelayCommand _upCommand;
        public RelayCommand UpCommand
        {
            get
            {
                return _upCommand ??
                    (
                        _upCommand = new RelayCommand
                        (
                            () =>
                            {
                                Value++;
                                if (Value > MaxValue)
                                    Value = MaxValue;
                            }
                        )
                    );
            }
        }

        private RelayCommand _downCommand;
        public RelayCommand DownCommand
        {
            get
            {
                return _downCommand ??
                    (
                        _downCommand = new RelayCommand
                        (
                            () =>
                            {
                                Value--;
                                if (Value < MinValue)
                                    Value = MinValue;
                            }
                        )
                    );
            }
        }

        public NumberTextbox()
        {
            InitializeComponent();
        }
    }
}
