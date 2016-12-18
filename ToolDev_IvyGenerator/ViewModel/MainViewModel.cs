using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using SharpDX;
using DaeSharpWpf;
using DaeSharpWPF;
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
                                    _models.Add(ModelLoader.LoadModel(dlg.FileName, control.GetDevice()));
                                    RaisePropertyChanged("Models");
                                }
                                else
                                    Debug.WriteLine("Nope");
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
                                       //First create a pick ray. Need projection matrix, 
                                       var mousePos = Mouse.GetPosition(control);
                                       //SharpDX.Ray ray = Ray.GetPickRay((int)mousePos.X, (int)mousePos.Y, control.Viewport, null);
                                       var ray = cam.GetPickRay((float)mousePos.X, (float)mousePos.Y);
                                       //Do the models
                                       //Debug.WriteLine((Models as Model).Intersects(ray));
                                   }
                                   
                               }
                           )
                       );
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

        private List<IModel<VertexPosColNorm>> _models = new List<IModel<VertexPosColNorm>>();
        public IModel<VertexPosColNorm>[] Models
        {
            get { return _models.ToArray(); }
            set
            {
                //_models = value;
                //RaisePropertyChanged("Models");
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