using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ToolDev_IvyGenerator.Models;
using SharpDX;
using System;
using System.Windows.Controls;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator.ViewModel
{

    public class IvyParametersViewModel : ViewModelBase
    {
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
                                IvyData.AddCp(cp);
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
                                IvyData.DeleteCp();
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

                                //Add new model
                                if ((bool)result)
                                {
                                    IvyData.LeafModel = new Model();
                                    IvyData.LeafModel.Mesh = ModelLoader<VertexPosColNorm>.LoadModel(dlg.FileName, mvm.Device);
                                    IvyData.LeafModel.Initialize(mvm.Device);
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
                                IvyData.LeafModel = null;
                            }
                        )
                    );
            }
        }

        private RelayCommand _generateRandomValuesCommand;
        public RelayCommand GenerateRandomValuesCommand
        {
            get
            {
                return _generateRandomValuesCommand ??
                    (
                        _generateRandomValuesCommand = new RelayCommand
                        (
                            () =>
                            {
                                int frequency = Convert.ToInt32(1.0 / IvyData.LeafSpreadInterval);
                                if (IvyData.Symmetrical)
                                    frequency *= 2;

                                IvyData.Ivy.RefreshRandomValues(frequency * (IvyData.ControlPoints.Count - 1));
                            }
                        )
                    );
            }
        }

        private IvyViewModel _ivyData;
        public IvyViewModel IvyData
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

        //private Ivy _ivyData;
        //public Ivy IvyData
        //{
        //    get
        //    {
        //        return _ivyData;
        //    }
        //    set
        //    {
        //        _ivyData = value;
        //        RaisePropertyChanged("IvyData");
        //    }
        //}

        public IvyParametersViewModel()
        {
        }
    }
}