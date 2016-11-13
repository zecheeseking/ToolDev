using System;
using System.Windows;

namespace WarSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {

        CommandCenter _cc = new CommandCenter();

        public MainWindow()
        {
            InitializeComponent();
            _cc.CreateTeams();
            this.DataContext = _cc;
        }

        private void Btn_FightClick(object sender, RoutedEventArgs e)
        {
            _cc.Fight();
        }

        private void Btn_Reset(object sender, RoutedEventArgs e)
        {
            _cc.CreateTeams();
        }
    }
}
