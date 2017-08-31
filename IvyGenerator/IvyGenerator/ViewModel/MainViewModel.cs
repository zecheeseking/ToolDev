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
using System.Windows.Media.Imaging;

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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

        private RelayCommand<TextBox>changeAngleCommand;
        public RelayCommand<TextBox> ChangeAngleCommand
        {
            get
            {
                return changeAngleCommand ??
                       (
                           changeAngleCommand = new RelayCommand<TextBox>
                           (
                               (tb) =>
                               {
                                   try
                                   {
                                       float newValue =
                                           float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       if (Tree.Angle != newValue)
                                       {
                                           History.Do(new SaveTreeMemento(Tree));
                                           Tree.Angle =
                                               float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       }
                                   }
                                   catch (FormatException ex)
                                   {
                                       MessageBox.Show("Please enter a valid value!");
                                       tb.Text = Tree.Angle.ToString();
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<TextBox> changeRadiusCommand;
        public RelayCommand<TextBox> ChangeRadiusCommand
        {
            get
            {
                return changeRadiusCommand ??
                       (
                           changeRadiusCommand = new RelayCommand<TextBox>
                           (
                               (tb) =>
                               {
                                   try
                                   {
                                       float newValue =
                                           float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       if (Tree.Radius != newValue)
                                       {
                                           History.Do(new SaveTreeMemento(Tree));
                                           Tree.Radius =
                                               float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       }
                                   }
                                   catch (FormatException ex)
                                   {
                                       MessageBox.Show("Please enter a valid value!");
                                       tb.Text = Tree.Radius.ToString();
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<TextBox> changeRadiusReductionCommand;
        public RelayCommand<TextBox> ChangeRadiusReductionCommand
        {
            get
            {
                return changeRadiusReductionCommand ??
                       (
                           changeRadiusReductionCommand = new RelayCommand<TextBox>
                           (
                               (tb) =>
                               {
                                   try
                                   {
                                       float newValue =
                                           float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       if (Tree.RadiusReduction != newValue)
                                       {
                                           History.Do(new SaveTreeMemento(Tree));
                                           Tree.RadiusReduction =
                                               float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       }
                                   }
                                   catch (FormatException ex)
                                   {
                                       MessageBox.Show("Please enter a valid value!");
                                       tb.Text = Tree.RadiusReduction.ToString();
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<TextBox> changeLengthCommand;
        public RelayCommand<TextBox> ChangeLengthCommand
        {
            get
            {
                return changeLengthCommand ??
                       (
                           changeLengthCommand = new RelayCommand<TextBox>
                           (
                               (tb) =>
                               {
                                   try
                                   {
                                       float newValue =
                                           float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       if (Tree.Length != newValue)
                                       {
                                           History.Do(new SaveTreeMemento(Tree));
                                           Tree.Length =
                                               float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       }
                                   }
                                   catch (FormatException ex)
                                   {
                                       MessageBox.Show("Please enter a valid value!");
                                       tb.Text = Tree.Length.ToString();
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<TextBox> changeTreeLimbSidesCommand;
        public RelayCommand<TextBox> ChangeTreeLimbSidesCommand
        {
            get
            {
                return changeTreeLimbSidesCommand ??
                       (
                           changeTreeLimbSidesCommand = new RelayCommand<TextBox>
                           (
                               (tb) =>
                               {
                                   try
                                   {
                                       float newValue =
                                           float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       if (Tree.TreeLimbSides != newValue)
                                       {
                                           History.Do(new SaveTreeMemento(Tree));
                                           Tree.TreeLimbSides =
                                               Int32.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       }
                                   }
                                   catch (FormatException ex)
                                   {
                                       MessageBox.Show("Please enter a valid value!");
                                       tb.Text = Tree.TreeLimbSides.ToString();
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<TextBox> changeBranchInterpolationPointsCommand;
        public RelayCommand<TextBox> ChangeBranchInterpolationPointsCommand
        {
            get
            {
                return changeBranchInterpolationPointsCommand ??
                       (
                           changeBranchInterpolationPointsCommand = new RelayCommand<TextBox>
                           (
                               (tb) =>
                               {
                                   try
                                   {
                                       float newValue =
                                           float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       if (Tree.BranchInterpolationPoints != newValue)
                                       {
                                           History.Do(new SaveTreeMemento(Tree));
                                           Tree.BranchInterpolationPoints =
                                               Int32.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       }
                                   }
                                   catch (FormatException ex)
                                   {
                                       MessageBox.Show("Please enter a valid value!");
                                       tb.Text = Tree.BranchInterpolationPoints.ToString();
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<TextBox> changeMinLeavesCommand;
        public RelayCommand<TextBox> ChangeMinLeavesCommand
        {
            get
            {
                return changeMinLeavesCommand ??
                       (
                           changeMinLeavesCommand = new RelayCommand<TextBox>
                           (
                               (tb) =>
                               {
                                   try
                                   {
                                       float newValue =
                                           float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       if (Tree.MinLeaves != newValue)
                                       {
                                           History.Do(new SaveTreeMemento(Tree));
                                           Tree.MinLeaves =
                                               Int32.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       }
                                   }
                                   catch (FormatException ex)
                                   {
                                       MessageBox.Show("Please enter a valid value!");
                                       tb.Text = Tree.MinLeaves.ToString();
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<TextBox> changeMaxLeavesCommand;
        public RelayCommand<TextBox> ChangeMaxLeavesCommand
        {
            get
            {
                return changeMaxLeavesCommand ??
                       (
                           changeMaxLeavesCommand = new RelayCommand<TextBox>
                           (
                               (tb) =>
                               {
                                   try
                                   {
                                       float newValue = float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       if (Tree.MaxLeaves != newValue)
                                       {
                                           History.Do(new SaveTreeMemento(Tree));
                                           Tree.MaxLeaves = Int32.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       }
                                   }
                                   catch (FormatException ex)
                                   {
                                       MessageBox.Show("Please enter a valid value!");
                                       tb.Text = Tree.MaxLeaves.ToString();
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<TextBox> changeMinLeafScaleCommand;
        public RelayCommand<TextBox> ChangeMinLeafScaleCommand
        {
            get
            {
                return changeMinLeafScaleCommand ??
                       (
                           changeMinLeafScaleCommand = new RelayCommand<TextBox>
                           (
                               (tb) =>
                               {
                                   try
                                   {
                                       float newValue = float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       if (Tree.MinLeafScale != newValue)
                                       {
                                           History.Do(new SaveTreeMemento(Tree));
                                           Tree.MinLeafScale = Single.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       }
                                   }
                                   catch (FormatException ex)
                                   {
                                       MessageBox.Show("Please enter a valid value!");
                                       tb.Text = Tree.MinLeafScale.ToString();
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<TextBox> changeMaxLeafScaleCommand;
        public RelayCommand<TextBox> ChangeMaxLeafScaleCommand
        {
            get
            {
                return changeMaxLeafScaleCommand ??
                       (
                           changeMaxLeafScaleCommand = new RelayCommand<TextBox>
                           (
                               (tb) =>
                               {
                                   try
                                   {
                                       float newValue = float.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       if (Tree.MaxLeafScale != newValue)
                                       {
                                           History.Do(new SaveTreeMemento(Tree));
                                           Tree.MaxLeafScale = Single.Parse(tb.Text, System.Globalization.CultureInfo.InvariantCulture);
                                       }
                                   }
                                   catch (FormatException ex)
                                   {
                                       MessageBox.Show("Please enter a valid value!");
                                       tb.Text = Tree.MaxLeafScale.ToString();
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
                                        string s = ObjExporter.MeshesToString("Tree", new [] { tree.TreeGeometry, tree.LeavesGeometry}, ModelTransform);
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

        private RelayCommand loadLeafTextureCommand;
        public RelayCommand LoadLeafTextureCommand
        {
            get
            {
                return loadLeafTextureCommand ??
                       (
                           loadLeafTextureCommand = new RelayCommand(
                               () =>
                               {
                                   var d = new OpenFileDialog()
                                   {
                                       Filter = "image files|*.jpg; *.png; *.bmp; *.gif",
                                   };
                                   if (d.ShowDialog().Value)
                                   {
                                       if (File.Exists(d.FileName))
                                       {
                                           var img = new BitmapImage(new Uri(d.FileName, UriKind.RelativeOrAbsolute));
                                           //this.TryGetExif(d.FileName);
                                           //this.SetImages(img);
                                           //this.Title = d.FileName;

                                           var mat = new PhongMaterial()
                                           {
                                               AmbientColor = Color.Gray,
                                               DiffuseColor = Color.White,
                                               SpecularColor = Color.White,
                                               DiffuseAlphaMap = new MemoryStream(img.ToByteArray()),
                                               DiffuseMap = new MemoryStream(img.ToByteArray())
                                           };

                                           Tree.LeafMaterial = mat;
                                       }
                                   }
                               }
                           )
                       );
            }
        }

        private RelayCommand<MenuItem> refreshLocalLSystemsCommand;

        public RelayCommand<MenuItem> RefreshLocalLSystemsCommand
        {
            get
            {
                return refreshLocalLSystemsCommand ??
                       (
                           refreshLocalLSystemsCommand = new RelayCommand<MenuItem>(
                               (menu) =>
                               {
                                   menu.Items.Clear();
                                   if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Resources/"))
                                       Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "Resources/");

                                   var files = Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory + "Resources/");
                                   var formatter = new BinaryFormatter();
                                   foreach (var s in files)
                                   {
                                       if(!s.EndsWith(".LRules"))
                                           continue;
                                       
                                       var byteStream = File.ReadAllBytes(s);
                                       if (byteStream == null) continue;
                                       using (var ms = new MemoryStream(byteStream))
                                       {
                                           var ruleSet = (RuleSet)formatter.Deserialize(ms);
                                           MenuItem item = new MenuItem();
                                           item.Header = "Create " + s.Substring(s.LastIndexOf("/") + 1, s.LastIndexOf(".") - s.LastIndexOf("/") - 1);
                                           item.Click += (sender, e) =>
                                           {
                                               DoCommand.Execute(item);
                                               Tree.SetRuleSet(ruleSet);
                                           };

                                           menu.Items.Add(item);
                                       }
                                   }
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