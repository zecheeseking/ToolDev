using System.Windows;
using DirectxWpf.Effects;
using DirectxWpf.Models;
using SharpDX;
using SharpDX.Direct3D10;
using Format = SharpDX.DXGI.Format;
using DaeSharpWpf;

namespace ToolDev_IvyGenerator
{
    class DX10Viewport : IDX10Viewport
    {
        private Device1 _device;
        private RenderTargetView _renderTargetView;
        private DX10RenderCanvas _renderControl;

        public IModel Model { get; set; }
        public IEffect Shader { get; set; }
		
		private float _modelRotation;

        public void Initialize(Device1 device, RenderTargetView renderTarget, DX10RenderCanvas canvasControl)
        {
            _device = device;
            _renderTargetView = renderTarget;
            _renderControl = canvasControl;

            //Set Model (IModel)
			//Set Shader (IEffect)
        }

        public void Deinitialize()
        {
            
        }

        public void Update(float deltaT)
        {
            if (Model != null && Shader != null)
            {
                _modelRotation += MathUtil.PiOverFour*deltaT;

                var worldMat = Matrix.Identity;
                worldMat *= Matrix.Scaling(20.0f);
                worldMat *= Matrix.RotationY(_modelRotation);

                var viewMat = Matrix.LookAtLH(new Vector3(0, 50, -100), Vector3.Zero, Vector3.UnitY);
                var projMat = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, (float)_renderControl.ActualWidth / (float)_renderControl.ActualHeight, 0.1f, 1000f);

                Shader.SetWorld(worldMat);
                Shader.SetWorldViewProjection(worldMat*viewMat*projMat);
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
    }
}
