using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using System.Windows.Controls;
using ToolDev_IvyGenerator.Models;
using System.Collections.Generic;
using SharpDX;
using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Diagnostics;
using System.Windows.Data;

namespace ToolDev_IvyGenerator.ViewModel
{
    public class CreateIvyViewModel : ViewModelBase
    {
        public List<SplineControlPoint> ControlPoints
        {
            get
            {
                return CreatedIvy.Stem.ControlPoints;
            }
            set
            {
                CreatedIvy.Stem.ControlPoints = value;
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
                                if (CreatedIvy.Stem.ControlPoints.Count >= 2)
                                {
                                    var dataContext = window.DataContext as MainViewModel;
                                    CreatedIvy.Stem.InterpolationSteps = InterpSteps;
                                    CreatedIvy.Stem.Sides = Sides;
                                    CreatedIvy.Stem.Thickness = (float)SplineThickness;

                                    CreatedIvy.Initialize(dataContext.Device);
                                    dataContext.Models.Add(CreatedIvy);
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

        private RelayCommand<ListBox> _addCPCommand;
        public RelayCommand<ListBox> AddCPCommand
        {
            get
            {
                return _addCPCommand ??
                    (
                        _addCPCommand = new RelayCommand<ListBox>
                        (
                            (listbox) =>
                            {
                                if (CreatedIvy.Stem.ControlPoints.Count == 0)
                                    CreatedIvy.Stem.ControlPoints.Add(
                                        new SplineControlPoint(CreatedIvy.Stem.ControlPoints.Count, 
                                        Vector3.Zero, Vector3.Right * 10f));
                                else
                                {
                                    var lastCp = CreatedIvy.Stem.ControlPoints[CreatedIvy.Stem.ControlPoints.Count - 1];
                                    CreatedIvy.Stem.ControlPoints.Add(new SplineControlPoint(CreatedIvy.Stem.ControlPoints.Count, 
                                        lastCp.Position.Value + (Vector3.Right * 50), 
                                        lastCp.Position.Value + (Vector3.Right * 50) + (Vector3.Right * 10f)));
                                }

                                listbox.Items.Refresh();
                            }
                        )
                    );
            }
        }

        private RelayCommand<ListBox> _deleteCPCommand;
        public RelayCommand<ListBox> DeleteCPCommand
        {
            get
            {
                return _deleteCPCommand ??
                    (
                        _deleteCPCommand = new RelayCommand<ListBox>
                        (
                            (listbox) =>
                            {
                                if(CreatedIvy.Stem.ControlPoints.Count > 0)
                                    CreatedIvy.Stem.ControlPoints.RemoveAt(CreatedIvy.Stem.ControlPoints.Count - 1);

                                listbox.Items.Refresh();
                            }
                        )
                    );
            }
        }

        private Ivy _createdIvy;
        public Ivy CreatedIvy
        {
            get
            {
                if (_createdIvy == null)
                    _createdIvy = new Ivy();

                return _createdIvy;
            }

            set
            {
                _createdIvy = value;
                RaisePropertyChanged("CreatedIvy");
            }
        }

        public CreateIvyViewModel()
        {
        }
    }
}