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
            _renderControl.Camera.Initialize((float)_renderControl.ActualWidth, (float)_renderControl.ActualHeight);

            //WORLD MATRIX SETUP
            _worldMatrix = Matrix.Identity;
            _worldMatrix *= Matrix.Scaling(1.0f);
            _worldMatrix *= Matrix.RotationY(0.0f);

            Shader = new PosNormColEffect();
            Shader.Create(device);
        }

        public void Deinitialize()
        {
            
        }

        public void Update(float deltaT)
        {
            _renderControl.Camera.Update(deltaT);

            if (_renderControl.Model != null && Shader != null)
            {
                Shader.SetWorld((_renderControl.Model as Model).WorldMatrix);
                Shader.SetWorldViewProjection(_worldMatrix * _renderControl.Camera.ViewMatrix * _renderControl.Camera.ProjectionMatrix);
            }
        }

        public void Render(float deltaT)
        {
            if (_device == null)
                return;

            if (_renderControl.Model != null && Shader != null)
            {
                _device.InputAssembler.InputLayout = Shader.InputLayout;
                _device.InputAssembler.PrimitiveTopology = _renderControl.Model.PrimitiveTopology;
                _device.InputAssembler.SetIndexBuffer(_renderControl.Model.IndexBuffer, Format.R32_UInt, 0);
                _device.InputAssembler.SetVertexBuffers(0,
                    new VertexBufferBinding(_renderControl.Model.VertexBuffer, _renderControl.Model.VertexStride, 0));

                for (int i = 0; i < Shader.Technique.Description.PassCount; ++i)
                {
                    Shader.Technique.GetPassByIndex(i).Apply();
                    _device.DrawIndexed(_renderControl.Model.IndexCount, 0, 0);
                }
            }
        }

        public Device1 GetDevice()
        {
            return _device;
        }
    }
}
