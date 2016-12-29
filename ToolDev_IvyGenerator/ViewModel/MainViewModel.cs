using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using System.Windows.Input;
using SharpDX;
using ToolDev_IvyGenerator.DirectX;
using ToolDev_IvyGenerator.Interfaces;
using ToolDev_IvyGenerator.Models;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        private RelayCommand<Dx10RenderCanvas> _loadModelCommand;
        public RelayCommand<Dx10RenderCanvas> LoadModelCommand
        {
            get
            {
                return _loadModelCommand ??
                    (
                        _loadModelCommand = new RelayCommand<Dx10RenderCanvas>
                        (
                            (control) =>
                            {
                                var dlg = new Microsoft.Win32.OpenFileDialog();
                                dlg.DefaultExt = ".obj";
                                dlg.Filter = "OBJ Files|*.obj|FBX Files|*.fbx|Overload Files|*.ovm";

                                bool? result = dlg.ShowDialog();

                                ////Add new model
                                if ((bool) result)
                                {
                                    var obj = new Model();
                                    obj.Mesh = ModelLoader<VertexPosColNorm>.LoadModel(dlg.FileName, control.GetDevice());
                                    obj.Initialize(control.GetDevice());
                                    Models.Add(obj);
                                    SelectedModel = Models[Models.Count - 1];
                                }
                            }
                        )
                    );
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
                                           {
                                               newModel = model as ISceneObject;
                                               //var newModel = new Model(model);
                                               //newModel.Position = new Vector3(hitPoint.X, hitPoint.Y, hitPoint.Z);
                                               //newModel.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                                               //toAdd.Add(newModel);
                                           }
                                           else
                                           {
                                               model.ResetCollisionFlags();
                                           }

                                       }
                                       SelectedModel = newModel;
                                       //foreach (Model m in toAdd)
                                       //{
                                       //     Models.Add(m);
                                       //}
                                   }
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
                _selectedModel = value;
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

        private RelayCommand<Dx10RenderCanvas> _createTestSplineCommand;
        public RelayCommand<Dx10RenderCanvas> CreateTestSplineCommand
        {
            get
            {
                return _createTestSplineCommand ??
                    (
                        _createTestSplineCommand = new RelayCommand<Dx10RenderCanvas>
                        (
                            (control) =>
                            {
                                var testSpline = new Spline();
                                testSpline.Initialize(control.GetDevice());
                                Models.Add(testSpline);
                                SelectedModel = Models[Models.Count - 1];
                            }
                        )
                    );
            }
        }

        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }
    }
}