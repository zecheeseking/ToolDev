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
    public class IvyParametersViewModel : ViewModelBase
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

                                var lastCp = IvyData.Stem.ControlPoints[IvyData.Stem.ControlPoints.Count - 1];
                                var cp = new SplineControlPoint(IvyData.Stem.ControlPoints.Count,
                                    lastCp.Position.Value + (Vector3.Right * 50), lastCp.Position.Value + (Vector3.Right * 50) + (Vector3.Right * 10f));
                                cp.Initialize(dc.Device);
                                IvyData.Stem.ControlPoints.Add(cp);
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

                                if (IvyData.Stem.ControlPoints.Count > 0)
                                    IvyData.Stem.ControlPoints.RemoveAt(IvyData.Stem.ControlPoints.Count - 1);
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
                                    //IvyData.LeafModel = new Model();
                                    //IvyData.LeafModel.Mesh = ModelLoader<VertexPosColNorm>.LoadModel(dlg.FileName, mvm.Device);
                                    //IvyData.LeafModel.Initialize(mvm.Device);
                                    //IvyData.PopulateLeaves();
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
                                //IvyData.LeafModel = null;
                            }
                        )
                    );
            }
        }

        private Ivy _ivyData;
        public Ivy IvyData
        {
            get
            {
                return _ivyData;
            }
            set
            {
                _ivyData = value;
                RaisePropertyChanged("IvyData");
            }
        }

        public IvyParametersViewModel()
        {
        }
    }
}