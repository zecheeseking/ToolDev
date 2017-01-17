using System.Windows;
using System.Windows.Controls;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator.CustomControls
{
    public partial class ControlPointListBox : UserControl
    {
        public static DependencyProperty PositionProperty = DependencyProperty.Register(
        "Position", typeof(Vec3), typeof(ControlPointListBox), new FrameworkPropertyMetadata(new Vec3()));

        public Vec3 Position
        {
            get { return (Vec3)GetValue(PositionProperty); }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        public static DependencyProperty TangentProperty = DependencyProperty.Register(
        "Tangent", typeof(Vec3), typeof(ControlPointListBox), new FrameworkPropertyMetadata(new Vec3()));

        public Vec3 Tangent
        {
            get { return (Vec3)GetValue(TangentProperty); }
            set
            {
                SetValue(TangentProperty, value);
            }
        }

        public ControlPointListBox()
        {
            InitializeComponent();
        }
    }
}
