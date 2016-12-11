using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using System.Diagnostics;

using DaeSharpWpf;
using SharpDX;
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

                                if ((bool) result)
                                    (control.Viewport as DX10Viewport).Model = ModelLoader.LoadModel(dlg.FileName, control.GetDevice());
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
                                   Debug.WriteLine("Narf");
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