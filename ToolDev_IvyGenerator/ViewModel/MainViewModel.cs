using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using SharpDX;
using ToolDev_IvyGenerator.DirectX;
using ToolDev_IvyGenerator.Interfaces;
using ToolDev_IvyGenerator.Models;
using ToolDev_IvyGenerator.Utilities;
using ToolDev_IvyGenerator.View;
using Device = SharpDX.Direct3D10.Device1;
using SharpDX.Direct3D10;

namespace ToolDev_IvyGenerator.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private RelayCommand _loadModelCommand;
        public RelayCommand LoadModelCommand
        {
            get
            {
                return _loadModelCommand ??
                    (
                        _loadModelCommand = new RelayCommand
                        (
                            () =>
                            {
                                var dlg = new Microsoft.Win32.OpenFileDialog();
                                dlg.DefaultExt = ".obj";
                                dlg.Filter = "OBJ Files|*.obj|FBX Files|*.fbx|Overload Files|*.ovm";

                                bool? result = dlg.ShowDialog();

                                ////Add new model
                                if ((bool) result)
                                {
                                    var obj = new Model();
                                    obj.Mesh = ModelLoader<VertexPosColNorm>.LoadModel(dlg.FileName, Device);
                                    obj.Initialize(Device);
                                    Models.Add(obj);
                                    SelectedModel = Models[Models.Count - 1];
                                }
                            }
                        )
                    );
            }
        }

        private Device _device;
        public Device Device
        {
            get
            {
                if (_device == null)
                    _device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_1);

                return _device;
            }
        }

        private RelayCommand<Dx10RenderCanvas> _raycastCommand;
        public RelayCommand<Dx10RenderCanvas> RaycastCommand
        {
            get
            {
                return _raycastCommand ??
                       (
                           _raycastCommand = new RelayCommand<Dx10RenderCanvas>
                           (
                               (control) =>
                               {
                                   var cam = Camera as Camera;
                                   if (cam != null && !cam.MovementEnabled)
                                   {
                                       var mousePos = Mouse.GetPosition(control);
                                       var ray = cam.GetPickRay((float)mousePos.X, (float)mousePos.Y);
                                       ISceneObject newModel = null;
                                       foreach (var m in Models)
                                       {
                                           var model = m as IIntersect;
                                           if (model == null)
                                               break;

                                           Vector3 hitPoint;

                                           if (model.Intersects(ray, out hitPoint))
                                               newModel = model as ISceneObject;
                                           else
                                               model.ResetCollisionFlags();
                                       }

                                       if(SelectedModel != newModel && _lockSelection != true)
                                            SelectedModel = newModel;

                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand _raycastResetCommand;
        public RelayCommand RaycastResetCommand
        {
            get
            {
                return _raycastResetCommand ??
                       (
                           _raycastResetCommand = new RelayCommand
                           (
                               () =>
                               {
                                   //var m = SelectedModel as IIntersect;
                                   //if (m != null)
                                   //    m.ResetCollisionFlags();
                               }
                           )
                       );
            }
        }

        private bool _lockSelection;
        public bool LockSelection
        {
            get { return _lockSelection; }
            set
            {
                _lockSelection = value;
                RaisePropertyChanged("LockSelection");
            }
        }

        private RelayCommand<Button> _lockSelectionCommand;
        public RelayCommand<Button> LockSelectionCommand
        {
            get
            {
                return _lockSelectionCommand ??
                       (
                           _lockSelectionCommand = new RelayCommand<Button>
                           (
                               (button) =>
                               {
                                   LockSelection = !LockSelection;

                                   if (LockSelection)
                                       button.Background = Brushes.CornflowerBlue;
                                   else
                                       button.ClearValue(Button.BackgroundProperty);
                               }
                           )
                       );
            }
        }

        private ISceneObject _selectedModel;
        public ISceneObject SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                if (_selectedModel != null)
                    (_selectedModel as IIntersect).Selected = false;
                _selectedModel = value;
                if(_selectedModel != null)
                    (_selectedModel as IIntersect).Selected = true;
                RaisePropertyChanged("SelectedModel");
                RaisePropertyChanged("ObjectSelected");
            }
        }

        public bool ObjectSelected
        {
            get
            {
                if (_selectedModel == null) return false;
                return true;
            }
        }

        private RelayCommand<Dx10RenderCanvas> _windowResizedCommand;
        public RelayCommand<Dx10RenderCanvas> WindowResizedCommand
        {
            get
            {
                return _windowResizedCommand ??
                       (
                           _windowResizedCommand = new RelayCommand<Dx10RenderCanvas>
                           (
                               (control) =>
                               {
                                   (Camera as Camera).SetScreenWidthHeight((float)control.ActualWidth, (float)control.ActualHeight);
                               }
                           )
                       );
            }
        }

        private RelayCommand<bool> _viewportCameraToggleCommand;
        public RelayCommand<bool> ViewportCameraToggleCommand
        {
            get
            {
                return _viewportCameraToggleCommand ??
                    (
                        _viewportCameraToggleCommand = new RelayCommand<bool>
                        (
                            (toggle) =>
                            {
                                (Camera as Camera).MovementEnabled = toggle;
                            }
                        )
                    );
            }
        }

        private List<ISceneObject> _models = new List<ISceneObject>();
        public List<ISceneObject> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                RaisePropertyChanged("Models");
            }
        }

        private ICamera _camera;
        public ICamera Camera
        {
            get
            {
                if(_camera == null)
                    _camera = new Camera();

                return _camera;
            }
            set
            {
                _camera = value;
                RaisePropertyChanged("Camera");
            }
        }

        private RelayCommand<Window> _createIvyCommand;
        public RelayCommand<Window> CreateIvyCommand
        {
            get
            {
                return _createIvyCommand ??
                    (
                        _createIvyCommand = new RelayCommand<Window>
                        (
                            (window) =>
                            {
                                //var createIvyView = new CreateIvyView();
                                //createIvyView.Owner = window;
                                //createIvyView.Show();
                            }
                        )
                    );
            }
        }

        public MainViewModel()
        {

        }
    }
}