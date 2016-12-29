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
    public class SplineViewModel : ViewModelBase
    {
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

        /// <summary>
        /// Initializes a new instance of the SplineViewModel class.
        /// </summary>
        public SplineViewModel()
        {
        }
    }
}