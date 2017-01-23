using GalaSoft.MvvmLight;
using ToolDev_IvyGenerator.Models;
using System.Collections.ObjectModel;
using Device = SharpDX.Direct3D10.Device1;

namespace ToolDev_IvyGenerator.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class IvyViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the IvyViewModel class.
        /// </summary>
        private readonly Ivy _ivy;
        public Ivy Ivy
        { get { return _ivy; } }

        public Spline Stem
        {
            get { return _ivy.Stem; }
            set { _ivy.Stem = value; RaisePropertyChanged(() => Stem); }
        }

        public Model LeafModel
        {
            get { return _ivy.LeafModel; }
            set { _ivy.LeafModel = value; _ivy.PopulateLeaves();
                RaisePropertyChanged(() => LeafModel);
                RaisePropertyChanged(() =>LeafModelRotX);
                RaisePropertyChanged(() => LeafModelRotY);
                RaisePropertyChanged(() => LeafModelRotZ);
                RaisePropertyChanged(() => LeafModelScaleX);
                RaisePropertyChanged(() => LeafModelScaleY);
                RaisePropertyChanged(() => LeafModelScaleZ);

            }
        }

        public float LeafOffset
        {
            get { return _ivy.Offset; }
            set { _ivy.Offset = value; _ivy.PopulateLeaves(); RaisePropertyChanged(() => LeafOffset); }
        }

        public float LeafModelRotX
        {
            get { if(_ivy.LeafModel != null) return _ivy.LeafModel.Rotation.X; return 0.0f; }
            set { _ivy.LeafModel.Rotation.X = value; _ivy.PopulateLeaves(); RaisePropertyChanged(() => LeafModelRotX); }
        }

        public float LeafModelRotY
        {
            get { if (_ivy.LeafModel != null) return _ivy.LeafModel.Rotation.Y; return 0.0f; }
            set { _ivy.LeafModel.Rotation.Y = value; _ivy.PopulateLeaves(); RaisePropertyChanged(() => LeafModelRotY); }
        }

        public float LeafModelRotZ
        {
            get { if (_ivy.LeafModel != null) return _ivy.LeafModel.Rotation.Z; return 0.0f; }
            set { _ivy.LeafModel.Rotation.Z = value; _ivy.PopulateLeaves(); RaisePropertyChanged(() => LeafModelRotZ); }
        }

        public float LeafModelScaleX
        {
            get { if (_ivy.LeafModel != null) return _ivy.LeafModel.Scale.X; return 0.0f; }
            set { _ivy.LeafModel.Scale.X = value; _ivy.PopulateLeaves(); RaisePropertyChanged(() => LeafModelScaleX); }
        }

        public float LeafModelScaleY
        {
            get { if (_ivy.LeafModel != null) return _ivy.LeafModel.Scale.Y; return 0.0f; }
            set { _ivy.LeafModel.Scale.Y = value; _ivy.PopulateLeaves(); RaisePropertyChanged(() => LeafModelScaleY); }
        }

        public float LeafModelScaleZ
        {
            get { if (_ivy.LeafModel != null) return _ivy.LeafModel.Scale.Z; return 0.0f; }
            set { _ivy.LeafModel.Scale.Z = value; _ivy.PopulateLeaves(); RaisePropertyChanged(() => LeafModelScaleZ); }
        }

        public float MaxLeafIntervalRange
        {
            get { return _ivy.MaxIntervalRange; }
            set { _ivy.MaxIntervalRange = value; RaisePropertyChanged(() => MaxLeafIntervalRange); }
        }

        public float MinLeafIntervalRange
        {
            get { return _ivy.MinIntervalRange; }
            set { _ivy.MinIntervalRange = value; RaisePropertyChanged(() => MinLeafIntervalRange); }
        }

        public float MaxRandomRotationRange
        {
            get { return _ivy.MaxRotationRange; }
            set { _ivy.MaxRotationRange = value; RaisePropertyChanged(() => MaxRandomRotationRange); }
        }

        public float MinRandomRotationRange
        {
            get { return _ivy.MinRotationRange; }
            set { _ivy.MinRotationRange = value;  RaisePropertyChanged(() => MinRandomRotationRange); }
        }

        public float MaxRangeScalingRange
        {
            get { return _ivy.MaxScalingRange; }
            set { _ivy.MaxScalingRange = value;  RaisePropertyChanged(() => MaxRangeScalingRange); }
        }

        public float MinRangeScalingRange
        {
            get { return _ivy.MinScalingRange; }
            set { _ivy.MinScalingRange = value; RaisePropertyChanged(() => MinRangeScalingRange); }
        }

        public bool Render
        {
            get { return _ivy.Stem.Render; }
            set { _ivy.Stem.Render = value; RaisePropertyChanged(() => Render); }
        }

        public bool Symmetrical
        {
            get { return _ivy.Symmetrical; }
            set { _ivy.Symmetrical = value; _ivy.PopulateLeaves(); RaisePropertyChanged(() => Symmetrical); }
        }

        public int StemInterpolationSteps
        {
            get { return _ivy.Stem.InterpolationSteps; }
            set { _ivy.Stem.InterpolationSteps = value; RaisePropertyChanged(() => StemInterpolationSteps); }
        }

        public int StemSides
        {
            get { return _ivy.Stem.Sides; }
            set { _ivy.Stem.Sides = value; RaisePropertyChanged(() => StemSides); }
        }

        public float StemThickness
        {
            get { return _ivy.Stem.Thickness; }
            set { _ivy.Stem.Thickness = value; RaisePropertyChanged(() => StemThickness); }
        }

        public float LeafSpreadInterval
        {
            get { return _ivy.LeafInterval; }
            set { _ivy.LeafInterval = value;  RaisePropertyChanged(() => LeafSpreadInterval); }
        }

        

        public ObservableCollection<SplineControlPoint> ControlPoints
        {
            get
            {
                return new ObservableCollection<SplineControlPoint>(_ivy.Stem.ControlPoints);
            }
        }

        public void AddCp(SplineControlPoint cp)
        {
            _ivy.Stem.ControlPoints.Add(cp);
            RaisePropertyChanged(() => ControlPoints);
        }

        public void DeleteCp()
        {
            if (_ivy.Stem.ControlPoints.Count > 0)
                _ivy.Stem.ControlPoints.RemoveAt(_ivy.Stem.ControlPoints.Count - 1);
            RaisePropertyChanged(() => ControlPoints);
        }

        public IvyViewModel(Ivy ivy)
        {
            _ivy = ivy;
        }
    }
}