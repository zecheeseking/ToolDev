using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using HelixToolkit.Wpf.SharpDX;
using System.Windows.Media.Media3D;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using SharpDX;
using System.Collections.Generic;
using System.IO;
using IvyGenerator.Model;
using IvyGenerator.View;
using ObjExporter = IvyGenerator.Utilities.ObjExporter;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using System.Runtime.Serialization.Formatters.Binary;

using System.Diagnostics;
using System.Windows.Forms;
using IvyGenerator.Utilities;

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

        public UndoRedoManager<Tree> History { get; private set; }

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

        private Tree tree = new Tree();
        public Tree Tree
        {
            get { return tree; }
            set
            {
                tree = value;
                RaisePropertyChanged("Tree");
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
                                   History.Do(new SaveTreeMemento(Tree));
                                   Tree.Generate();
                                   RaisePropertyChanged("Tree");
                               }
                           )
                       );
            }
        }

        private RelayCommand resetLSystemCommand;
        public RelayCommand ResetLSystemCommand
        {
            get
            {
                return resetLSystemCommand ??
                       (
                           resetLSystemCommand = new RelayCommand
                           (
                               () =>
                               {
                                   Tree.Reset();
                                   RaisePropertyChanged("Tree");
                               }
                           )
                       );
            }
        }

        private RelayCommand<string> changeAngleCommand;
        public RelayCommand<string> ChangeAngleCommand
        {
            get
            {
                return changeAngleCommand ??
                       (
                           changeAngleCommand = new RelayCommand<string>
                           (
                               (s) =>
                               {
                                    float newValue = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
                                    if (Tree.Angle != newValue)
                                    {
                                       History.Do(new SaveTreeMemento(Tree));
                                       Tree.Angle = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
                                    }
                               }
                           )
                       );
            }
        }

        private RelayCommand<string> changeRadiusCommand;
        public RelayCommand<string> ChangeRadiusCommand
        {
            get
            {
                return changeRadiusCommand ??
                       (
                           changeRadiusCommand = new RelayCommand<string>
                           (
                               (s) =>
                               {
                                   float newValue = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
                                   if (Tree.Radius != newValue)
                                   {
                                       History.Do(new SaveTreeMemento(Tree));
                                       Tree.Radius = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<string> changeRadiusReductionCommand;
        public RelayCommand<string> ChangeRadiusReductionCommand
        {
            get
            {
                return changeRadiusReductionCommand ??
                       (
                           changeRadiusReductionCommand = new RelayCommand<string>
                           (
                               (s) =>
                               {
                                   float newValue = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
                                   if (Tree.RadiusReduction != newValue)
                                   {
                                       History.Do(new SaveTreeMemento(Tree));
                                       Tree.RadiusReduction = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<string> changeLengthCommand;
        public RelayCommand<string> ChangeLengthCommand
        {
            get
            {
                return changeLengthCommand ??
                       (
                           changeLengthCommand = new RelayCommand<string>
                           (
                               (s) =>
                               {
                                   float newValue = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
                                   if (Tree.Length != newValue)
                                   {
                                       History.Do(new SaveTreeMemento(Tree));
                                       Tree.Length = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand openRulesetWizardCommand;
        public RelayCommand OpenRulesetWizardCommand
        {
            get
            {
                return openRulesetWizardCommand ??
                       (
                           openRulesetWizardCommand = new RelayCommand
                           (
                               () =>
                               {
                                    CreateRuleSetWindow window = new CreateRuleSetWindow();
                                   window.Show();
                               }
                           )
                       );
            }
        }

        private RelayCommand<string> testCommand;
        public RelayCommand<string> TestCommand
        {
            get
            {
                return testCommand ??
                       (
                           testCommand = new RelayCommand<string>
                           (
                               (s) =>
                               {
                                   Debug.WriteLine(s);
                               }
                           )
                       );
            }
        }

        private RelayCommand loadProjectCommand;
        public RelayCommand LoadProjectCommand
        {
            get
            {
                return loadProjectCommand ??
                       (
                           loadProjectCommand = new RelayCommand
                           (
                               () =>
                               {
                                   OpenFileDialog openFile = new OpenFileDialog();
                                   openFile.Filter = "Branch Generator Project Files (*.LSysTree)|*.LSysTree|All files (*.*)|*.*";

                                   if (openFile.ShowDialog() == true)
                                   {
                                       using (FileStream fs = new FileStream(openFile.FileName, FileMode.Open))
                                       {
                                           BinaryFormatter bs = new BinaryFormatter();
                                           SaveTreeMemento obj = (SaveTreeMemento) bs.Deserialize(fs);
                                           Tree.LoadProject(obj);
                                           Tree.Generate(false);
                                       }
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand saveProjectCommand;
        public RelayCommand SaveProjectCommand
        {
            get
            {
                return saveProjectCommand ??
                       (
                           saveProjectCommand = new RelayCommand
                           (
                               () =>
                               {
                                   SaveFileDialog saveFile = new SaveFileDialog();
                                   saveFile.FileName = "tree.LSysTree";
                                   saveFile.Filter = "Branch Generator Project Files (*.LSysTree)|*.LSysTree|All files (*.*)|*.*";

                                   if (saveFile.ShowDialog() == true)
                                   {
                                       SaveTreeMemento obj = new SaveTreeMemento(tree);

                                       using (var stream = File.Create(saveFile.FileName))
                                       {
                                           var formatter = new BinaryFormatter();
                                           formatter.Serialize(stream, obj);
                                       }
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand exportModelCommand;
        public RelayCommand ExportModelCommand
        {
            get
            {
                return exportModelCommand ??
                       (
                           exportModelCommand = new RelayCommand
                           (
                               () =>
                               {
                                   SaveFileDialog saveFileDialog = new SaveFileDialog();
                                   saveFileDialog.FileName = "Tree.obj";
                                   saveFileDialog.Filter = "OBJ files (*.obj)|*.obj|All files (*.*)|*.*";
                                   saveFileDialog.FilterIndex = 0;

                                   if (saveFileDialog.ShowDialog() == true)
                                   {
                                        string s = ObjExporter.MeshToString("Tree", tree.TreeGeometry, ModelTransform);
                                        File.WriteAllText(saveFileDialog.FileName, s);
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand doCommand;
        public RelayCommand DoCommand
        {
            get
            {
                return doCommand ??
                       (
                           doCommand = new RelayCommand(
                               () =>
                               {
                                   History.Do(new SaveTreeMemento(tree));
                               }
                           )
                       );
            }
        }

        private RelayCommand undoCommand;
        public RelayCommand UndoCommand
        {
            get
            {
                return undoCommand ??
                    (
                        undoCommand = new RelayCommand(
                            () =>
                            {
                                if(History.CanUndo)
                                    History.Undo();
                            }
                        )
                    );
            }
        }

        private RelayCommand redoCommand;
        public RelayCommand RedoCommand
        {
            get
            {
                return redoCommand ??
                       (
                           redoCommand = new RelayCommand(
                               () =>
                               {
                                   History.Redo();
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
            GridTransform = new TranslateTransform3D(-5, 0, -5);

            this.ModelGeometry = new Element3DCollection();
            this.ModelTransform = new TranslateTransform3D(0, 0, 0);

            tree = new Tree();
            History = new UndoRedoManager<Tree>(tree);
        }
    }
}