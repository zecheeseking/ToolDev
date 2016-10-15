using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Pong
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            pongControl.ScoreChanged += (l, r) =>
            {
                lblLeftScore.Content = l.ToString(CultureInfo.InvariantCulture);
                lblRightScore.Content = r.ToString(CultureInfo.InvariantCulture);
            };
        }
    }
}