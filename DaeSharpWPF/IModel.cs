using System.ComponentModel;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using SharpDX.Direct3D9;
using Buffer = SharpDX.Direct3D10.Buffer;
using SharpDX;

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
    }
}