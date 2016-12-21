using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using DaeSharpWpf;
using DaeSharpWpf.Interfaces;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using ToolDev_IvyGenerator.Effects;
using ToolDev_IvyGenerator.Structs;
using Buffer = SharpDX.Direct3D10.Buffer;
using Device = SharpDX.Direct3D10.Device1;

namespace ToolDev_IvyGenerator.Models
{
    class Spline : ISceneObject, INotifyPropertyChanged
    {
        public Matrix WorldMatrix { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public IEffect Material { get; set; }

        public Vector3 LightDirection { get; set; }

        public MeshData<VertexPosColNorm> Mesh { get; set; }

        private List<SplineControlPoint> _controlPoints = new List<SplineControlPoint>();
        public int InterpolationSteps { get; set; }

        public Spline()
        {
            WorldMatrix = Matrix.Scaling(1.0f) * Matrix.RotationQuaternion(Quaternion.Identity) * Matrix.Translation(Vector3.Zero);

            _controlPoints.Add(new SplineControlPoint(Vector3.Zero, Vector3.Zero + Vector3.Up * 10.0f));
            _controlPoints.Add(new SplineControlPoint(new Vector3(100, 50, 0), new Vector3(100, 50, 0) + Vector3.Down * 10.0f));

            Mesh = new MeshData<VertexPosColNorm>();

            Mesh.PrimitiveTopology = PrimitiveTopology.LineList;
            Mesh.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));
            InterpolationSteps = 10;
            Mesh.IndexCount = (InterpolationSteps - 1) * 2;

            Mesh.Positions = new Vector3[InterpolationSteps];
            Mesh.Normals = new Vector3[InterpolationSteps];
            Mesh.Positions[0] = _controlPoints[0].Position;

            for (int i = 1; i < InterpolationSteps - 1; ++i)
            {
                var t = (float)i / (InterpolationSteps - 1);
                Mesh.Positions[i] = CalculateSplinePoint(t, _controlPoints[0], _controlPoints[1]);
                Mesh.Normals[i] = Vector3.Up;
            }
            Mesh.Positions[Mesh.Positions.Length - 1] = _controlPoints[1].Position;
            Mesh.Normals[Mesh.Normals.Length - 1] = Vector3.Up;

            Mesh.Vertices = new VertexPosColNorm[Mesh.Positions.Length + 1];
            for (int i = 0; i < Mesh.Positions.Length; ++i)
                Mesh.Vertices[i] = new VertexPosColNorm(Mesh.Positions[i], Color.Gray, Mesh.Normals[i]);

            Mesh.Vertices[Mesh.Vertices.Length - 1] = new VertexPosColNorm(_controlPoints[1].Position, Color.Pink, Vector3.Up);

            Mesh.Indices = new uint[Mesh.IndexCount];
            Mesh.Indices[0] = 0;
            Mesh.Indices[1] = 1;
            for (int i = 2; i < Mesh.IndexCount; i += 2)
            {
                Mesh.Indices[i] = Mesh.Indices[i - 1];
                Mesh.Indices[i + 1] = Mesh.Indices[i - 1] + 1;
            }
        }

        public void Initialize(Device device)
        {
            Material = new SceneGridEffect();
            Material.Create(device);

            Mesh.CreateVertexBuffer(device);
            Mesh.CreateIndexBuffer(device);
        }

        public void Update(float deltaT)
        {
            //throw new NotImplementedException();
        }

        public void Draw(Device device, ICamera camera)
        {
            Material.SetWorldViewProjection(WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix);

            device.InputAssembler.InputLayout = Material.InputLayout;
            device.InputAssembler.PrimitiveTopology = Mesh.PrimitiveTopology;
            device.InputAssembler.SetIndexBuffer(Mesh.IndexBuffer, Format.R32_UInt, 0);
            device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(Mesh.VertexBuffer, Mesh.VertexStride, 0));

            for (int i = 0; i < Material.Technique.Description.PassCount; ++i)
            {
                Material.Technique.GetPassByIndex(i).Apply();
                device.DrawIndexed(Mesh.IndexCount, 0, 0);
            }
        }

        public Vector3 CalculateSplinePoint(float t, SplineControlPoint p0, SplineControlPoint p1)
        {
            Vector3 sp = (float)(2.0f * Math.Pow(t, 3.0f) - 3.0f * Math.Pow(t, 2.0f) + 1) * p0.Position +
                (float)(Math.Pow(t, 3.0f) - 2.0f * Math.Pow(t, 2.0f) + t) * p0.Tangent +
                (float)(-2.0f * Math.Pow(t, 3.0f) + 3.0f * Math.Pow(t, 2.0f)) * p1.Position +
                (float)(Math.Pow(t, 3.0f) - Math.Pow(t, 2.0f)) * p1.Tangent;

            return sp;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
