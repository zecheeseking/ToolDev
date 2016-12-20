using System.Windows.Input;
using DaeSharpWpf;
using ToolDev_IvyGenerator.Effects;
using SharpDX;
using SharpDX.Direct3D10;
using ToolDev_IvyGenerator.Models;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator
{
    class DX10Viewport : IDX10Viewport
    {
        private Device1 _device;
        private RenderTargetView _renderTargetView;
        private Dx10RenderCanvas _renderControl;

        public IEffect Shader { get; set; }
		
        private Matrix _worldMatrix;

        private SceneGrid _grid;

        private Vector3 _lightDirection;

        private Spline _testSpline;

        public void Initialize(Device1 device, RenderTargetView renderTarget, Dx10RenderCanvas canvasControl)
        {
            _device = device;
            _renderTargetView = renderTarget;
            _renderControl = canvasControl;

            //CAMERA SETUP
            _renderControl.Camera.Initialize((float)_renderControl.ActualWidth, (float)_renderControl.ActualHeight);
            _lightDirection = (_renderControl.Camera as Camera).CameraForward;

            //WORLD MATRIX SETUP
            _worldMatrix = Matrix.Identity;
            _worldMatrix *= Matrix.Scaling(1.0f);
            _worldMatrix *= Matrix.RotationY(0.0f);

            _grid = new SceneGrid();
            _grid.Initialize(device);

            _testSpline = new Spline();
            _testSpline.Initialize(_device);
        }

        public void Deinitialize()
        {
            
        }

        public void Update(float deltaT)
        {
            //FIX THIS
            //if ((_renderControl.Camera as Camera).MovementEnabled)
            //    Mouse.Capture(_renderControl);
            //else
            //    Mouse.Capture(null);

            _renderControl.Camera.Update(deltaT);
        }

        public void Render(float deltaT)
        {
            if (_device == null)
                return;

            _grid.Draw(_device, _renderControl.Camera, _lightDirection);

            if (_renderControl.Models.Count != 0 && Shader != null)
            {
                foreach (IModel<VertexPosColNorm> m in _renderControl.Models)
                {
                    var model = m as Model;

                    if (model == null)
                        break;

                    m.Draw(_device, _renderControl.Camera, _lightDirection);
                }
            }

            _testSpline.Draw(_device, _renderControl.Camera, _lightDirection);
        }

        public Device1 GetDevice()
        {
            return _device;
        }
    }
}
