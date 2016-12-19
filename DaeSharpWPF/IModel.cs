using SharpDX.Direct3D;
using Buffer = SharpDX.Direct3D10.Buffer;
using Device = SharpDX.Direct3D10.Device;

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
        void CreateVertexBuffer(Device device);
        void CreateIndexBuffer(Device device);
    }
}