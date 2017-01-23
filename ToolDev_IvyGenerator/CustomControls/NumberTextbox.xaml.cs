using System.Windows;
using System.Diagnostics;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Input;

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
                if (value < MinValue)
                    SetValue(ValueProperty, MinValue);
                else if (value > MaxValue)
                    SetValue(ValueProperty, MaxValue);
                else
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

        public static readonly DependencyProperty IncrementValueProperty = DependencyProperty.Register(
        "IncrementValue", typeof(double), typeof(NumberTextbox), new FrameworkPropertyMetadata(1.0));

        public double IncrementValue
        {
            get { return (double)GetValue(IncrementValueProperty); }
            set
            {
                SetValue(IncrementValueProperty, value);
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
                                Value += IncrementValue;
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
                                Value -= IncrementValue;
                                if (Value < MinValue)
                                    Value = MinValue;
                            }
                        )
                    );
            }
        }

        private RelayCommand<TextBox> _textboxClearCommand;
        public RelayCommand<TextBox> TextboxClearCommand
        {
            get
            {
                return _textboxClearCommand ??
                    (
                        _textboxClearCommand = new RelayCommand<TextBox>
                        (
                            (textbox) =>
                            {
                                Debug.WriteLine("Haha");
                                textbox.Text = "";
                            }
                        )
                    );
            }
        }

    private void NumberValidationTextbox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    public NumberTextbox()
        {
            InitializeComponent();
        }
    }
}
