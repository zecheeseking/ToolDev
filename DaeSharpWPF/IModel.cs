using DaeSharpWPF;
using SharpDX.Direct3D;
using SharpDX;
using Buffer = SharpDX.Direct3D10.Buffer;
using Device = SharpDX.Direct3D10.Device1;

namespace DaeSharpWpf
{
    public interface IModel<T>
    {
        PrimitiveTopology PrimitiveTopology { get; set; }
        int VertexStride { get; set; }
        int IndexCount { get; set; }
        T[] Vertices { get; set; }
        uint[] Indices { get; set; }
        Buffer IndexBuffer { get; set; }
        Buffer VertexBuffer { get; set; }
        IEffect Material { get; set; }
        void CreateVertexBuffer(Device device);
        void CreateIndexBuffer(Device device);
        void Draw(Device device, ICamera camera, Vector3 lightDirection);
    }
}