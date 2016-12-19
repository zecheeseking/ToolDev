using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DaeSharpWpf;
using DaeSharpWPF;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using ToolDev_IvyGenerator.Structs;
using Buffer = SharpDX.Direct3D10.Buffer;
using Device = SharpDX.Direct3D10.Device1;

namespace ToolDev_IvyGenerator.Models
{
    class Spline : IModel<VertexPosCol>
    {
        public PrimitiveTopology PrimitiveTopology { get; set; }
        public int VertexStride { get; set; }
        public int IndexCount { get; set; }
        public VertexPosCol[] Vertices { get; set; }
        public uint[] Indices { get; set; }
        public Buffer IndexBuffer { get; set; }
        public Buffer VertexBuffer { get; set; }
        public IEffect Material { get; set; }



        private List<SplineControlPoint> _controlPoints = new List<SplineControlPoint>();

        public int InterpolationSteps { get; set; }

        public Spline(Device device)
        {
            _controlPoints.Add(new SplineControlPoint(Vector3.Zero, Vector3.Zero + Vector3.Right));
            _controlPoints.Add(new SplineControlPoint(new Vector3(100, 50, 0), new Vector3(100, 50, 0) + Vector3.Right));

            PrimitiveTopology = PrimitiveTopology.LineList;
            VertexStride = Marshal.SizeOf(typeof(VertexPosCol));
            InterpolationSteps = 10;
            IndexCount = InterpolationSteps*2;

            Vertices = new VertexPosCol[InterpolationSteps];
            Vertices[0] = new VertexPosCol(_controlPoints[0].Position, Color.Pink);
            int count = 1;
            for (int i = 1; i < InterpolationSteps - 1; ++i)
            {
                var t = (float)i / (InterpolationSteps - 1);
                var p = CalculateSplinePoint(t, _controlPoints[i - 1], _controlPoints[i]);
                Vertices[count] = new VertexPosCol(p, Color.Pink);
                count++;
            }
            Vertices[Vertices.Length - 1] = new VertexPosCol(_controlPoints[1].Position, Color.Pink);

            CreateVertexBuffer(device);
            CreateIndexBuffer(device);
        }

        public Vector3 CalculateSplinePoint(float t, SplineControlPoint p0, SplineControlPoint p1)
        {
            Vector3 sp = (float)(2.0f * Math.Pow(t, 3.0f) - 3.0f * Math.Pow(t, 2.0f) + 1) * p0.Position +
                (float)(Math.Pow(t, 3.0f) - 2.0f * Math.Pow(t, 2.0f) + t) * p0.Tangent +
                (float)(-2.0f * Math.Pow(t, 3.0f) + 3.0f * Math.Pow(t, 2.0f)) * p1.Position +
                (float)(Math.Pow(t, 3.0f) - Math.Pow(t, 2.0f)) * p1.Tangent;

            return sp;
        }

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


        public void Draw(Device device, ICamera camera, Vector3 lightDirection)
        {
            throw new NotImplementedException();
        }

        public void Draw(Device device)
        {
            
        }
    }
}
