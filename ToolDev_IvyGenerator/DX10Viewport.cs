using System.Windows;
using System.Windows.Input;
using DaeSharpWpf;
using ToolDev_IvyGenerator.Effects;
using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DirectInput;
using ToolDev_IvyGenerator.Models;
using ToolDev_IvyGenerator.Utilities;
using Format = SharpDX.DXGI.Format;

namespace ToolDev_IvyGenerator
{
    class DX10Viewport : IDX10Viewport
    {
        private Device1 _device;
        private RenderTargetView _renderTargetView;
        private Dx10RenderCanvas _renderControl;
        private Camera _camera;

        public IModel Model { get; set; }
        public IEffect Shader { get; set; }
		
		private float _modelRotation;
        private Matrix _worldMatrix;

        public void Initialize(Device1 device, RenderTargetView renderTarget, Dx10RenderCanvas canvasControl)
        {
            _device = device;
            _renderTargetView = renderTarget;
            _renderControl = canvasControl;

            //CAMERA SETUP
            _camera = new Camera((float)_renderControl.ActualWidth, (float)_renderControl.ActualHeight);
            _camera.SetPosition(new Vector3(0,50,-100));

            //WORLD MATRIX SETUP
            _worldMatrix = Matrix.Identity;
            _worldMatrix *= Matrix.Scaling(1.0f);
            _worldMatrix *= Matrix.RotationY(45.0f);

            Shader = new PosNormColEffect();
            Shader.Create(device);
        }

        public void Deinitialize()
        {
            
        }

        public void Update(float deltaT)
        {
            if (Model != null && Shader != null)
            {
                //var viewMat = Matrix.LookAtLH(new Vector3(0, 50, -100), new Vector3(0, 15, 0), Vector3.UnitY);
                var viewMat = Matrix.LookAtLH(_camera.Position, Vector3.Zero, Vector3.UnitY);

                Shader.SetWorld(_worldMatrix);
                Shader.SetWorldViewProjection(_worldMatrix * viewMat*_camera.ProjectionMatrix);
            }
        }

        public void Render(float deltaT)
        {
            if (_device == null)
                return;

            if (Model != null && Shader != null)
            {
                _device.InputAssembler.InputLayout = Shader.InputLayout;
                _device.InputAssembler.PrimitiveTopology = Model.PrimitiveTopology;
                _device.InputAssembler.SetIndexBuffer(Model.IndexBuffer, Format.R32_UInt, 0);
                _device.InputAssembler.SetVertexBuffers(0,
                    new VertexBufferBinding(Model.VertexBuffer, Model.VertexStride, 0));

                for (int i = 0; i < Shader.Technique.Description.PassCount; ++i)
                {
                    Shader.Technique.GetPassByIndex(i).Apply();
                    _device.DrawIndexed(Model.IndexCount, 0, 0);
                }
            }
        }

        public Device1 GetDevice()
        {
            return _device;
        }
    }
}
