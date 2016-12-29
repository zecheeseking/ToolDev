using System.Windows;
using System.Windows.Controls;
using ToolDev_IvyGenerator.Models;

namespace ToolDev_IvyGenerator.View
{
    public partial class ModelView : UserControl
    {
        public static readonly DependencyProperty ModelSourceProperty = DependencyProperty.Register(
        "ModelSource", typeof(Model), typeof(ModelView), new FrameworkPropertyMetadata(new Model()));

        public Model ModelSource
        {
            get { return (Model)GetValue(ModelSourceProperty); }
            set { SetValue(ModelSourceProperty, value); }
        }

        public ModelView()
        {
            InitializeComponent();
        }
    }
}
