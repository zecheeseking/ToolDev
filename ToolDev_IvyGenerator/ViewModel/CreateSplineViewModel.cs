using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using System.Windows.Controls;
using ToolDev_IvyGenerator.DirectX;
using ToolDev_IvyGenerator.Models;
using System.Diagnostics;
using System.Collections.Generic;
using SharpDX;
using System.Collections.ObjectModel;

namespace ToolDev_IvyGenerator.ViewModel
{
    public class CreateSplineViewModel : ViewModelBase
    {
        private ObservableCollection<SplineControlPoint> _controlPoints = new ObservableCollection<SplineControlPoint>();
        public ObservableCollection<SplineControlPoint> ControlPoints
        {
            get { return _controlPoints; }
            set
            {
                _controlPoints = value;
                RaisePropertyChanged("ControlPoints");
            }
        }

        private int _sides = 2;
        public int Sides
        {
            get { return _sides; }
            set
            {
                _sides = value;
                RaisePropertyChanged("Sides");
            }
        }

        private int _interpSteps = 3;
        public int InterpSteps
        {
            get { return _interpSteps; }
            set
            {
                _interpSteps = value;
                RaisePropertyChanged("InterpSteps");
            }
        }

        private double _splineThickness = 1.0;
        public double SplineThickness
        {
            get { return _splineThickness; }
            set
            {
                _splineThickness = value;
                RaisePropertyChanged("SplineThickness");
            }
        }

        private RelayCommand<Window> _createSplineCommand;
        public RelayCommand<Window> CreateSplineCommand
        {
            get
            {
                return _createSplineCommand ??
                    (
                        _createSplineCommand = new RelayCommand<Window>
                        (
                            (window) =>
                            {
                                if (_controlPoints.Count >= 2)
                                {
                                    var dataContext = window.DataContext as MainViewModel;
                                    var canvasControl = window.Owner.FindName("SceneWindow") as Dx10RenderCanvas;
                                    var spline = new Spline();

                                    spline.InterpolationSteps = InterpSteps;
                                    spline.Sides = Sides;
                                    spline.Thickness = (float)SplineThickness;
                                    List<SplineControlPoint> cps = new List<SplineControlPoint>();
                                    foreach (var cp in ControlPoints)
                                        cps.Add(cp);
                                    spline.ControlPoints = cps;

                                    spline.Initialize(canvasControl.GetDevice());
                                    dataContext.Models.Add(spline);
                                    dataContext.SelectedModel = dataContext.Models[dataContext.Models.Count - 1];
                                    window.Close();
                                }
                                else
                                {
                                    MessageBoxResult result = MessageBox.Show(
                                        "Cannot create a spline with less than 2 control points.\n Please add more control points.",
                                        "Creating Spline Error!", 
                                        MessageBoxButton.OK, 
                                        MessageBoxImage.Exclamation);
                                }
                            }
                        )
                    );
            }
        }

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

        private RelayCommand<Window> _quitWindowCommand;
        public RelayCommand<Window> QuitWindowCommand
        {
            get
            {
                return _quitWindowCommand ??
                    (
                        _quitWindowCommand = new RelayCommand<Window>
                        (
                            (window) =>
                            {
                                window.Close();
                            }
                        )
                    );
            }
        }

        private RelayCommand _addCPCommand;
        public RelayCommand AddCPCommand
        {
            get
            {
                return _addCPCommand ??
                    (
                        _addCPCommand = new RelayCommand
                        (
                            () =>
                            {
                                if (ControlPoints.Count == 0)
                                    ControlPoints.Add(new SplineControlPoint(_controlPoints.Count, Vector3.Zero, Vector3.Right * 10f));
                                else
                                {
                                    var lastCp = _controlPoints[_controlPoints.Count - 1];
                                    ControlPoints.Add(new SplineControlPoint(_controlPoints.Count, 
                                        lastCp.Position.Value + (Vector3.Right * 10), lastCp.Position.Value + (Vector3.Right * 10) + (Vector3.Right * 10f)));
                                }
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
                                if(ControlPoints.Count > 0)
                                    ControlPoints.RemoveAt(ControlPoints.Count - 1);
                            }
                        )
                    );
            }
        }

        public CreateSplineViewModel()
        {
        }
    }
}