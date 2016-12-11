using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using System.Diagnostics;

using System.Windows;
using SharpDX.Direct3D10;
using DaeSharpWpf;
using ToolDev_IvyGenerator.Utilities;
using ToolDev_IvyGenerator;

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
                                dlg.Filter = "OBJ Files (*.obj)|*.obj";

                                bool? result = dlg.ShowDialog();

                                if ((bool) result)
                                {
                                    var viewport = control.Viewport as DX10Viewport;
                                    viewport.Model = ModelLoader.LoadModel(dlg.FileName, control.GetDevice());
                                }
                                else
                                    Debug.WriteLine("Nope");
                            }
                        )
                    );
            }
        }

        public IModel Model { get; set; }

        public void Raycast()
        {
            Debug.WriteLine("Narf");
        }

        private void LoadModel(string path, Device device)
        {
            Model = ModelLoader.LoadModel(path, device);
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