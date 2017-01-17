using System.Windows;
using System.Windows.Controls;
using ToolDev_IvyGenerator.Models;
using ToolDev_IvyGenerator.ViewModel;

namespace ToolDev_IvyGenerator.View
{
    /// <summary>
    /// Interaction logic for ControlPointView.xaml
    /// </summary>
    public partial class ControlPointView : UserControl
    {
        private ControlPointViewModel _vm;

        public static readonly DependencyProperty ControlPointSourceProperty = DependencyProperty.Register(
        "ControlPointSource", typeof(SplineControlPoint), typeof(ControlPointView), new FrameworkPropertyMetadata(OnSplineCPSourceChanged));

        public SplineControlPoint ControlPointSource
        {
            get { return (SplineControlPoint)GetValue(ControlPointSourceProperty); }
            set
            {
                SetValue(ControlPointSourceProperty, value);
            }
        }

        private static void OnSplineCPSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ControlPointView)._vm.SplineCPData = e.NewValue as SplineControlPoint;
        }

        public ControlPointView()
        {
            InitializeComponent();
            _vm = RootCP.DataContext as ControlPointViewModel;
        }
    }
}
