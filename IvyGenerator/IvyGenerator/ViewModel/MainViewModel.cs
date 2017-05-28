using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using HelixToolkit.Wpf.SharpDX;
using System.Windows.Media.Media3D;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using SharpDX;

using Microsoft.Win32;
using System.Collections.Generic;
using IvyGenerator.Model;

namespace IvyGenerator.ViewModel
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
        private const string OpenFileFilter = "3D model files (*.obj;*.3ds)|*.obj;*.3ds";
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        private Camera camera;
        public Camera Camera
        {
            get { return camera; }
            set
            {
                camera = value;
                RaisePropertyChanged("Camera");
            }
        }

        public IRenderTechniquesManager RenderTechniquesManager { get; private set; }
        private RenderTechnique renderTechnique;
        public RenderTechnique RenderTechnique
        {
            get { return renderTechnique; }
            set
            {
                renderTechnique = value;
                RaisePropertyChanged("RenderTechnique");
            }
        }

        public IEffectsManager EffectsManager { get; private set; }

        public Vector3 DirectionalLightDirection { get; private set; }
        public Color4 DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }
        //Default grid info
        public LineGeometry3D Grid { get; private set; }
        public Transform3D GridTransform { get; private set; }
        public SharpDX.Color GridColor { get; private set; }

        public Element3DCollection ModelGeometry { get; private set; }
        public Transform3D ModelTransform { get; private set; }

        private RelayCommand<Viewport3DX> loadModelCommand;
        public RelayCommand<Viewport3DX> LoadModelCommand
        {
            get
            {
                return loadModelCommand ??
                       (
                           loadModelCommand = new RelayCommand<Viewport3DX>
                           (
                               (viewport) =>
                               {
                                   string path = OpenFileDialog(OpenFileFilter);
                                   if (path == null)
                                   {
                                       return;
                                   }

                                   var reader = new ObjReader();
                                   var objCol = reader.Read(path);
                                   AttachModelList(objCol, viewport);
                               }
                           )
                       );
            }
        }

        public void AttachModelList(List<Object3D> objs, Viewport3DX viewport)
        {
            this.ModelTransform = new TranslateTransform3D(0, 0, 0);
            this.ModelGeometry = new Element3DCollection();
            foreach (var ob in objs)
            {
                var s = new MeshGeometryModel3D
                {
                    Geometry = ob.Geometry,
                    Material = ob.Material,
                };
                this.ModelGeometry.Add(s);
                s.Attach(viewport.RenderHost);
            }
            RaisePropertyChanged("ModelGeometry");
        }

        private string OpenFileDialog(string filter)
        {
            var d = new OpenFileDialog();
            d.CustomPlaces.Clear();

            d.Filter = filter;

            if (!d.ShowDialog().Value)
                return null;

            return d.FileName;
        }

        private Tree ivy;
        public Tree Ivy
        {
            get { return ivy; }
            set
            {
                ivy = value;
                RaisePropertyChanged("Ivy");
            }
        }

        private RelayCommand advanceLSystemCommand;
        public RelayCommand AdvanceLSystemCommand
        {
            get
            {
                return advanceLSystemCommand ??
                       (
                           advanceLSystemCommand = new RelayCommand
                           (
                               () =>
                               {
                                   Ivy.Generate();
                                   RaisePropertyChanged("Ivy");
                               }
                           )
                       );
            }
        }

        public MainViewModel()
        {
            // camera setup
            Camera = new PerspectiveCamera
            {
                Position = new Point3D(3, 3, 5),
                LookDirection = new Vector3D(-3, -3, -5),
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 5000000
            };
            // default render technique
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);

            // setup lighting            
            AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            DirectionalLightColor = Color.White;
            DirectionalLightDirection = new Vector3(-2, -5, -2);

            // floor plane grid
            Grid = LineBuilder.GenerateGrid();
            GridColor = SharpDX.Color.Black;
            GridTransform = new TranslateTransform3D(-5, -1, -5);

            this.ModelGeometry = new Element3DCollection();
            this.ModelTransform = new TranslateTransform3D(0, 0, 0);

            ivy = new Tree();

            
        }
    }
}