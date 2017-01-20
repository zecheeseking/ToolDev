using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ToolDev_IvyGenerator.Models;
using SharpDX;
using System.Windows.Controls;
using ToolDev_IvyGenerator.Utilities;

using System.Diagnostics;

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

        private RelayCommand<UserControl> _addCPCommand;
        public RelayCommand<UserControl> AddCPCommand
        {
            get
            {
                return _addCPCommand ??
                    (
                        _addCPCommand = new RelayCommand<UserControl>
                        (
                            (userControl) =>
                            {
                                var dc = userControl.DataContext as MainViewModel;

                                var lastCp = SplineData.ControlPoints[SplineData.ControlPoints.Count - 1];
                                var cp = new SplineControlPoint(SplineData.ControlPoints.Count,
                                    lastCp.Position.Value + (Vector3.Right * 50), lastCp.Position.Value + (Vector3.Right * 50) + (Vector3.Right * 10f));
                                cp.Initialize(dc.Device);
                                SplineData.ControlPoints.Add(cp);
                            }
                        )
                    );
            }
        }

        private RelayCommand _deleteCPCommand;
        public RelayCommand DeleteCPCommand
        {
            get
            {
                return _deleteCPCommand ??
                    (
                        _deleteCPCommand = new RelayCommand
                        (
                            () =>
                            {

                                if (SplineData.ControlPoints.Count > 0)
                                    SplineData.ControlPoints.RemoveAt(SplineData.ControlPoints.Count - 1);
                            }
                        )
                    );
            }
        }

        private RelayCommand<MainViewModel> _loadLeafMeshCommand;
        public RelayCommand<MainViewModel> LoadLeafMeshCommand
        {
            get
            {
                return _loadLeafMeshCommand ??
                    (
                        _loadLeafMeshCommand = new RelayCommand<MainViewModel>
                        (
                            (mvm) =>
                            {
                                var dlg = new Microsoft.Win32.OpenFileDialog();
                                dlg.DefaultExt = ".obj";
                                dlg.Filter = "OBJ Files|*.obj|FBX Files|*.fbx|Overload Files|*.ovm";

                                bool? result = dlg.ShowDialog();

                                ////Add new model
                                if ((bool)result)
                                {
                                    SplineData.LeafModel = new Model();
                                    SplineData.LeafModel.Mesh = ModelLoader<VertexPosColNorm>.LoadModel(dlg.FileName, mvm.Device);
                                    SplineData.LeafModel.Initialize(mvm.Device);
                                    SplineData.PopulateLeaves();
                                }
                            }
                        )
                    );
            }
        }

        private RelayCommand _clearLeafMeshCommand;
        public RelayCommand ClearLeafMeshCommand
        {
            get
            {
                return _clearLeafMeshCommand ??
                    (
                        _clearLeafMeshCommand = new RelayCommand
                        (
                            () =>
                            {
                                SplineData.LeafModel = null;
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