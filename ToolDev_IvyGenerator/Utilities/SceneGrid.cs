
using System;
using System.Runtime.InteropServices;
using DaeSharpWpf;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using ToolDev_IvyGenerator.Effects;
using Buffer = SharpDX.Direct3D10.Buffer;

namespace ToolDev_IvyGenerator.Utilities
{
    public class SceneGrid : IModel<VertexPosCol>
    {
        public IEffect Shader { get; set; }
        public PrimitiveTopology PrimitiveTopology { get; set; }
        public int VertexStride { get; set; }
        public int IndexCount { get; set; }
        public VertexPosCol[] Vertices { get; set; }
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
            VertexBuffer = new Buffer(device, DataStream.Create(Vertices, false, false), bufferDescription);
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

        public void Initialize(Device device)
        {
            PrimitiveTopology = PrimitiveTopology.LineList;

            CreateGrid(20, Color.Black, 4.0f);

            CreateVertexBuffer(device);
            CreateIndexBuffer(device);
        }

        private void CreateGrid(int gridSize, Color color, float gridSpacing)
        {
            float zeroOffset = ((float)gridSize / 2) * gridSpacing;

            VertexStride = Marshal.SizeOf(typeof(VertexPosCol));
            Vertices = new VertexPosCol[((gridSize + 1) * 2) * 2];
            Indices = new uint[((gridSize + 1) * 2) * 2];

            for (int i = 0; i < Vertices.Length / 2; i += 2)
            {
                //Horizontal
                Vertices[i] = new VertexPosCol(new Vector3(0 - zeroOffset,0,0 - zeroOffset + i * (gridSpacing / 2)), color);
                Vertices[i + 1] = new VertexPosCol(new Vector3(0 + zeroOffset,0,0 - zeroOffset + i * (gridSpacing / 2)), color);
                Indices[i] = Convert.ToUInt32(i);
                Indices[i + 1] = Convert.ToUInt32(i + 1);
            }

            //vertical
            for (int i = Vertices.Length / 2; i < Vertices.Length; i += 2)
            {
                Vertices[i] = new VertexPosCol(new Vector3(0 - zeroOffset + (i - (Vertices.Length / 2)) * (gridSpacing / 2), 0,0 - zeroOffset), color);
                Vertices[i + 1] = new VertexPosCol(new Vector3(0 - zeroOffset + (i - (Vertices.Length / 2)) * (gridSpacing / 2), 0,0 + zeroOffset), color);
                Indices[i] = Convert.ToUInt32(i);
                Indices[i + 1] = Convert.ToUInt32(i + 1);
            }

            IndexCount = Indices.Length;
        }
    }
}