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
        "Value", typeof(double), typeof(NumberTextbox), new PropertyMetadata((System.Windows.PropertyChangedCallback)OnValueChanged));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
        "MaxValue", typeof(double), typeof(NumberTextbox), new FrameworkPropertyMetadata(10.0));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
        "MinValue", typeof(double), typeof(NumberTextbox), new FrameworkPropertyMetadata(0.0));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as NumberTextbox).Value = (double)e.NewValue;
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
