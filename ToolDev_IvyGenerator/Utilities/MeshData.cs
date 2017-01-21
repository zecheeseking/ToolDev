using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using Buffer = SharpDX.Direct3D10.Buffer;
using Device = SharpDX.Direct3D10.Device1;

namespace ToolDev_IvyGenerator.Utilities
{
    public class MeshData<T> where T : struct
    {
        public PrimitiveTopology PrimitiveTopology { get; set; }
        public int VertexStride { get; set; }
        public int IndexCount { get; set; }
        public Vector3[] Positions { get; set; }
        public Vector3[] Normals { get; set; }
        public Color[] Colors { get; set; }
        public T[] Vertices { get; set; }
        public uint[] Indices { get; set; }
        public Buffer IndexBuffer { get; set; }
        public Buffer VertexBuffer { get; set; }

        public void CreateVertexBuffer(Device device)
        {
            VertexBuffer?.Dispose();

            var bufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Immutable,
                SizeInBytes = VertexStride * Vertices.Length
            };

            VertexBuffer = new Buffer(device, DataStream.Create(Vertices, false, true), bufferDescription);
        }

        public void CreateIndexBuffer(Device device)
        {
            IndexBuffer?.Dispose();

            var bufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Immutable,
                SizeInBytes = sizeof(uint) * IndexCount
            };

            IndexBuffer = new Buffer(device, DataStream.Create(Indices, false, false), bufferDescription);
        }
    }
}
