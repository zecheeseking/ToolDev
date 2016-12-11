using System.ComponentModel;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using Buffer = SharpDX.Direct3D10.Buffer;

namespace DaeSharpWpf
{
    public interface IModel
    {
        PrimitiveTopology PrimitiveTopology { get; set; }
        int VertexStride { get; set; }
        int IndexCount { get; set; }
        Buffer IndexBuffer { get; set; }
        Buffer VertexBuffer { get; set; }
    }
}