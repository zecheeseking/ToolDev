using GalaSoft.MvvmLight;
using ToolDev_IvyGenerator.Models;

namespace ToolDev_IvyGenerator.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ControlPointViewModel : ViewModelBase
    {
        private SplineControlPoint _splineCPData;
        public SplineControlPoint SplineCPData
        {
            get
            {
                return _splineCPData;
            }
            set
            {
                _splineCPData = value;
                RaisePropertyChanged("SplineCPData");
            }
        }

        public ControlPointViewModel()
        {
        }
    }
}