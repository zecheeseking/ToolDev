using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ToolDev_IvyGenerator.Models;
using System.Windows.Controls;

namespace ToolDev_IvyGenerator.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SplineViewModel : ViewModelBase
    {
        private RelayCommand<ListBox> _refreshLBCommand;
        public RelayCommand<ListBox> RefreshLBCommand
        {
            get
            {
                return _refreshLBCommand ??
                    (
                        _refreshLBCommand = new RelayCommand<ListBox>
                        (
                            (listBox) =>
                            {
                                listBox.Items.Refresh();
                            }
                        )
                    );
            }
        }

        private Spline _splineData;
        public Spline SplineData
        {
            get
            {
                return _splineData;
            }
            set
            {
                _splineData = value;
                RaisePropertyChanged("SplineData");
            }
        }

        public SplineViewModel()
        {
        }
    }
}