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
                                if (IvyData.Stem.ControlPoints.Count >= 2)
                                {
                                    var dataContext = window.DataContext as MainViewModel;

                                    IvyData.Stem.Initialize(dataContext.Device);
                                    dataContext.Models.Add(IvyData.Ivy);
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
                                if (_ivyData.Stem.ControlPoints.Count == 0)
                                    _ivyData.AddCp(
                                        new SplineControlPoint(_ivyData.Stem.ControlPoints.Count, 
                                        Vector3.Zero, Vector3.Right * 10f));
                                else
                                {
                                    var lastCp = _ivyData.Stem.ControlPoints[_ivyData.Stem.ControlPoints.Count - 1];
                                    _ivyData.AddCp(new SplineControlPoint(_ivyData.Stem.ControlPoints.Count,
                                        lastCp.Position.Value + (Vector3.Right * 50),
                                        lastCp.Position.Value + (Vector3.Right * 50) + (Vector3.Right * 10f)));
                                }
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
                                IvyData.DeleteCp();
                            }
                        )
                    );
            }
        }

        private IvyViewModel _ivyData;
        public IvyViewModel IvyData
        {
            get { return _ivyData; }
            set
            {
                _ivyData = value;
                RaisePropertyChanged("IvyData");
            }
        }

        public CreateIvyViewModel()
        {
            _ivyData = new IvyViewModel(new Ivy());
        }
    }
}