using System;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;

namespace ToolDev_IvyGenerator.DirectX
{
    public struct AppContext
    {
        public Device _device;
        public DeviceContext _deviceContext;
        public ICamera camera;
    }

    public interface IDx10Viewport
    {
        void Initialize(AppContext appContext, RenderTargetView renderTarget, Dx10RenderCanvas canvasControl);
        void Deinitialize();
        void Update(float deltaT);
        void Render(float deltaT);
    }
}
