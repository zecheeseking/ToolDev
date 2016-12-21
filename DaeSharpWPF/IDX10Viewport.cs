using System;
using SharpDX.Direct3D10;
using Device = SharpDX.Direct3D10.Device1;

namespace DaeSharpWpf
{
    public interface IDx10Viewport
    {
        void Initialize(Device device, RenderTargetView renderTarget, Dx10RenderCanvas canvasControl);
        void Deinitialize();
        void Update(float deltaT);
        void Render(float deltaT);
    }
}
