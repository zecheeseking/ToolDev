using ToolDev_IvyGenerator.Interfaces;
using SharpDX;
using SharpDX.Direct3D11;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator.DirectX
{
    class DX10Viewport : IDx10Viewport
    {
        private AppContext _appContext;

        private RenderTargetView _renderTargetView;
        private Dx10RenderCanvas _renderControl;

        private Matrix _worldMatrix;

        private SceneGrid _grid;

        private Vector3 _lightDirection;


        public void Initialize(AppContext appContext, RenderTargetView renderTarget, Dx10RenderCanvas canvasControl)
        {
            _appContext = appContext;
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
            _grid.Initialize(_appContext._device);
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
            if (_appContext._device == null)
                return;

            _grid.Draw(_appContext);

            if (_renderControl.Models.Count != 0)
            {
                foreach (ISceneObject sObj in _renderControl.Models)
                {
                    //sObj.LightDirection = _lightDirection;
                    sObj.Draw(_appContext);
                }
            }
        }
    }
}
