using System.Windows;
using System.Windows.Input;
using DaeSharpWpf;
using ToolDev_IvyGenerator.Effects;
using SharpDX;
using SharpDX.Direct3D;
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

        public IEffect Shader { get; set; }
		
		private float _modelRotation;
        private Matrix _worldMatrix;

        private SceneGrid _grid;

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

            _grid = new SceneGrid();
            _grid.Initialize(device);
            _grid.Shader = new SceneGridEffect();
            _grid.Shader.Create(device);
        }

        public void Deinitialize()
        {
            
        }

        public void Update(float deltaT)
        {
            _renderControl.Camera.Update(deltaT);

            _grid.Shader.SetWorldViewProjection(_worldMatrix * _renderControl.Camera.ViewMatrix * _renderControl.Camera.ProjectionMatrix);

            ///FIX
            //if (_renderControl.Models != null && Shader != null)
            //{
            //    foreach (IModel m in _renderControl.Models)
            //    {
            //        Shader.SetWorld((m as Model).WorldMatrix);
            //        Shader.SetWorldViewProjection(_worldMatrix * _renderControl.Camera.ViewMatrix * _renderControl.Camera.ProjectionMatrix);
            //    }
            //}
        }

        public void Render(float deltaT)
        {
            if (_device == null)
                return;

            DrawGrid();

            ///FIX
            //if (_renderControl.Models != null && Shader != null)
            //{
            //    foreach (IModel m in _renderControl.Models)
            //    {
            //        _device.InputAssembler.InputLayout = Shader.InputLayout;
            //        _device.InputAssembler.PrimitiveTopology = m.PrimitiveTopology;
            //        _device.InputAssembler.SetIndexBuffer(m.IndexBuffer, Format.R32_UInt, 0);
            //        _device.InputAssembler.SetVertexBuffers(0,
            //            new VertexBufferBinding(m.VertexBuffer, m.VertexStride, 0));

            //        for (int i = 0; i < Shader.Technique.Description.PassCount; ++i)
            //        {
            //            Shader.Technique.GetPassByIndex(i).Apply();
            //            _device.DrawIndexed(m.IndexCount, 0, 0);
            //        }
            //    }
            //}
        }

        private void DrawGrid()
        {
            _device.InputAssembler.InputLayout = _grid.Shader.InputLayout;
            _device.InputAssembler.PrimitiveTopology = _grid.PrimitiveTopology;
            _device.InputAssembler.SetIndexBuffer(_grid.IndexBuffer, Format.R32_UInt, 0);
            _device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_grid.VertexBuffer, _grid.VertexStride, 0));

            for (int i = 0; i < _grid.Shader.Technique.Description.PassCount; ++i)
            {
                _grid.Shader.Technique.GetPassByIndex(i).Apply();
                _device.DrawIndexed(_grid.IndexCount, 0, 0);
            }
        }

        public Device1 GetDevice()
        {
            return _device;
        }
    }
}
