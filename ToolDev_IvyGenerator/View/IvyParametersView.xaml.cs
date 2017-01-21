using System.Windows;
using System.Windows.Controls;
using ToolDev_IvyGenerator.Models;
using ToolDev_IvyGenerator.ViewModel;

namespace ToolDev_IvyGenerator.View
{

    public partial class IvyParametersView : UserControl
    {
        private IvyParametersViewModel _vm;

        public static readonly DependencyProperty IvySourceProperty = DependencyProperty.Register(
        "IvySource", typeof(Ivy), typeof(IvyParametersView), new FrameworkPropertyMetadata(OnIvySourceChanged));

        public Spline IvySource
        {
            get { return (Spline)GetValue(IvySourceProperty); }
            set {
                SetValue(IvySourceProperty, value);
            }
        }

        private static void OnIvySourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as IvyParametersView)._vm.IvyData = e.NewValue as Ivy;
        }

        public IvyParametersView()
        {
            InitializeComponent();
            _vm = Root.DataContext as IvyParametersViewModel;
        }
    }
}
