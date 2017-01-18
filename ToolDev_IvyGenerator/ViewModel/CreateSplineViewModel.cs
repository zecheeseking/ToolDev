using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using ToolDev_IvyGenerator.DirectX;
using ToolDev_IvyGenerator.Models;
using System.Diagnostics;

namespace ToolDev_IvyGenerator.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CreateSplineViewModel : ViewModelBase
    {
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
                                var dataContext = window.DataContext as MainViewModel;
                                var spline = new Spline();
                                var canvasControl = window.Owner.FindName("SceneWindow") as Dx10RenderCanvas;
                                spline.Initialize(canvasControl.GetDevice());
                                dataContext.Models.Add(spline);
                                dataContext.SelectedModel = dataContext.Models[dataContext.Models.Count - 1];
                                window.Close();
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
        
        public CreateSplineViewModel()
        {
        }
    }
}