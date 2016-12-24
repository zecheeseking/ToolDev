using DaeSharpWpf;
using DaeSharpWpf.Interfaces;
using SharpDX;
using SharpDX.Direct3D10;
using ToolDev_IvyGenerator.Models;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator
{
    class DX10Viewport : IDx10Viewport
    {
        private Device1 _device;
        private RenderTargetView _renderTargetView;
        private Dx10RenderCanvas _renderControl;

        private Matrix _worldMatrix;

        private SceneGrid _grid;

        private Vector3 _lightDirection;


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

            //var _handle = new TransformHandle();
            //_handle.Initialize(device);
            //_renderControl.Models.Add(_handle);
        }

        public void Deinitialize()
        {
        }

        public void Update(float deltaT)
        {
            ////FIX THIS
            //if ((_renderControl.Camera as Camera).MovementEnabled)
            //    Mouse.Capture(_renderControl);
            //else
            //    Mouse.Capture(null);
            InputManager.Instance.Update();
            _renderControl.Camera.Update(deltaT);

            foreach (var sObj in _renderControl.Models)
            {
                sObj.Update(deltaT);
            }
        }

        public void Render(float deltaT)
        {
            if (_device == null)
                return;

            _grid.Draw(_device, _renderControl.Camera);

            if (_renderControl.Models.Count != 0)
            {
                foreach (ISceneObject sObj in _renderControl.Models)
                {
                    sObj.LightDirection = _lightDirection;
                    sObj.Draw(_device, _renderControl.Camera);
                }
            }
        }

        public Device1 GetDevice()
        {
            return _device;
        }
    }
}
