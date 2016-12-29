using System.Windows;
using System.Windows.Controls;
using ToolDev_IvyGenerator.Models;
using ToolDev_IvyGenerator.ViewModel;

namespace ToolDev_IvyGenerator.View
{

    public partial class SplineView : UserControl
    {
        private SplineViewModel _vm;

        public static readonly DependencyProperty SplineSourceProperty = DependencyProperty.Register(
        "SplineSource", typeof(Spline), typeof(SplineView), new FrameworkPropertyMetadata(OnSplineSourceChanged));

        public Spline SplineSource
        {
            get { return (Spline)GetValue(SplineSourceProperty); }
            set {
                SetValue(SplineSourceProperty, value);
            }
        }

        private static void OnSplineSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SplineView)._vm.SplineData = e.NewValue as Spline;
        }

        public SplineView()
        {
            InitializeComponent();
            _vm = Root.DataContext as SplineViewModel;
        }
    }
}
