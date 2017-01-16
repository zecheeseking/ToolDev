using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using ToolDev_IvyGenerator.Interfaces;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using ToolDev_IvyGenerator.Effects;
using ToolDev_IvyGenerator.Structs;
using ToolDev_IvyGenerator.Utilities;
using Buffer = SharpDX.Direct3D10.Buffer;
using Device = SharpDX.Direct3D10.Device1;

using System.Diagnostics;

namespace ToolDev_IvyGenerator.Models
{
    public class Spline : ISceneObject, INotifyPropertyChanged, IIntersect
    {
        private Matrix _worldMatrix;
        public Matrix WorldMatrix { get { return _worldMatrix; } set { _worldMatrix = value; } }
        public Vec3 Position { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; }

        public IEffect Material { get; set; }
        public IEffect WireMaterial { get; set; }

        public Vector3 LightDirection { get; set; }

        private bool _refreshBuffers = false;
        public bool Render { get; set; }

        public MeshData<VertexPosColNorm> Mesh { get; set; }
        public MeshData<VertexPosColNorm> WireMesh { get; set; }

        private List<SplineControlPoint> _controlPoints = new List<SplineControlPoint>();
        public List<SplineControlPoint> ControlPoints
        {
            get { return _controlPoints; }
        }

        private int _interpolationSteps;
        public int InterpolationSteps {
            get { return _interpolationSteps; }
            set
            {
                _interpolationSteps = value;
                PopulateSpline();
            }
        }

        private int _sides = 4;
        public int Sides
        {
            get { return _sides; }
            set
            {
                _sides = value;
                PopulateSpline();
            }
        }

        private float _thickness = 5.0f;

        public Spline()
        {
            _interpolationSteps = 4;

            Position = new Vec3 {Value = Vector3.Zero};
            Rotation = new Vec3 { Value = Vector3.Zero };
            Scale = new Vec3 { Value = new Vector3(1.0f) };

            WorldMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);

            _controlPoints.Add(new SplineControlPoint(0, Vector3.Zero, new Vector3(50,0,0)));
            _controlPoints.Add(new SplineControlPoint(1, new Vector3(50, 0, 50), new Vector3(0,0,50)));
            _controlPoints.Add(new SplineControlPoint(2, new Vector3(0, 0, 100), new Vector3(0,50,50)));
            _controlPoints.Add(new SplineControlPoint(3, new Vector3(50, 0, 150), new Vector3(0, 50, 50)));

            Mesh = new MeshData<VertexPosColNorm>();
            Mesh.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Mesh.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));

            WireMesh = new MeshData<VertexPosColNorm>();
            WireMesh.PrimitiveTopology = PrimitiveTopology.LineList;
            WireMesh.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));

            PopulateSpline();
            _refreshBuffers = false;
        }

        public void Initialize(Device device)
        {
            Material = new PosNormColEffect();
            Material.Create(device);

            WireMaterial = new SceneGridEffect();
            WireMaterial.Create(device);

            Mesh.CreateVertexBuffer(device);
            Mesh.CreateIndexBuffer(device);

            WireMesh.CreateVertexBuffer(device);
            WireMesh.CreateIndexBuffer(device);
        }

        public void Update(float deltaT)
        {
            //throw new NotImplementedException();
        }

        public void PopulateSpline()
        {
            PopulateWireSpline();
            PopulateMeshSpline();

            _refreshBuffers = true;
        }

        private void PopulateWireSpline()
        {
            //Position data
            WireMesh.Positions = new Vector3[_controlPoints.Count + (InterpolationSteps - 2) * (_controlPoints.Count - 1)];
            WireMesh.Normals = new Vector3[_controlPoints.Count + (InterpolationSteps - 2) * (_controlPoints.Count - 1)];

            for(int i = 0; i < _controlPoints.Count - 1; ++i)
            {
                for (int ii = 0; ii < InterpolationSteps - 1; ++ii)
                {
                    var t = (float)ii / (InterpolationSteps - 1);
                    WireMesh.Positions[i * (InterpolationSteps - 1) + ii] = Vector3.Hermite(_controlPoints[i].Position, _controlPoints[i].Tangent, _controlPoints[i+1].Position, _controlPoints[i+1].Tangent, t);
                    WireMesh.Normals[i * (InterpolationSteps - 1) + ii] = Vector3.Up;
                }
            }

            WireMesh.Positions[WireMesh.Positions.Length - 1] = _controlPoints[_controlPoints.Count - 1].Position;

            WireMesh.Vertices = new VertexPosColNorm[WireMesh.Positions.Length];
            for (int i = 0; i < WireMesh.Positions.Length; ++i)
                WireMesh.Vertices[i] = new VertexPosColNorm(WireMesh.Positions[i], Color.Pink);

            //INDICES
            WireMesh.IndexCount = WireMesh.Positions.Length * 2 - 2;

            WireMesh.Indices = new uint[WireMesh.IndexCount];
            WireMesh.Indices[0] = 0;
            WireMesh.Indices[1] = 1;

            for (int i = 2; i < WireMesh.IndexCount; i += 2)
            {
                WireMesh.Indices[i] = WireMesh.Indices[i - 1];
                WireMesh.Indices[i + 1] = WireMesh.Indices[i - 1] + 1;
            }
        }

        private void PopulateMeshSpline()
        {
            float angleIncrement = MathHelper.AngleToRadians(360.0f / _sides);

            Mesh.Positions = new Vector3[WireMesh.Positions.Length * _sides];
            Mesh.Colors = new Color[WireMesh.Positions.Length * _sides];
            Mesh.Normals = new Vector3[WireMesh.Positions.Length * _sides];

            for (int i = 0; i < WireMesh.Positions.Length; ++i)
            {
                Vector3 forward = Vector3.Zero;
                if (i == WireMesh.Positions.Length - 1)
                    forward = WireMesh.Positions[i] - WireMesh.Positions[i - 1];
                else
                    forward = WireMesh.Positions[i + 1] - WireMesh.Positions[i];

                forward.Normalize();

                var rotationMat = Matrix.RotationAxis(forward, angleIncrement);
                var lastPos = Vector3.Cross(forward, Vector3.Up);

                for (int ii = 0; ii < _sides; ++ii)
                {
                    var vec = Vector3.Zero;
                    Vector3.Transform(ref lastPos, ref rotationMat, out vec);
                    vec.Normalize();
                    Mesh.Positions[i * _sides + ii] = WireMesh.Positions[i] + (lastPos * _thickness);
                    var norm = WireMesh.Positions[i] - Mesh.Positions[i * _sides + ii];
                    norm.Normalize();
                    Mesh.Colors[i * _sides + ii] = Color.Azure;
                    Mesh.Normals[i * _sides + ii] = norm;
                    lastPos = new Vector3(vec.X, vec.Y, vec.Z);
                }
            }

            Mesh.Vertices = new VertexPosColNorm[Mesh.Positions.Length];

            for (int i = 0; i < Mesh.Positions.Length; ++i)
                Mesh.Vertices[i] = new VertexPosColNorm(Mesh.Positions[i], Mesh.Colors[i]);

            Mesh.IndexCount = _sides * 6 * (WireMesh.Positions.Length - 1);
            Mesh.Indices = new uint[Mesh.IndexCount];

            for(uint i = 0; i < WireMesh.Positions.Length - 1; ++i)
            {
                uint t = Convert.ToUInt32(_sides * 6 * i);

                uint startIndex = t;
                if(i == 0)
                    Mesh.Indices[startIndex] = startIndex;
                else
                    Mesh.Indices[startIndex] = Mesh.Indices[startIndex - 2];

                Mesh.Indices[startIndex + 1] = Mesh.Indices[startIndex] + 1;
                Mesh.Indices[startIndex + 2] = Convert.ToUInt32(Mesh.Indices[startIndex] + _sides);
                Mesh.Indices[startIndex + 3] = Mesh.Indices[startIndex + 1];
                Mesh.Indices[startIndex + 4] = Convert.ToUInt32(Mesh.Indices[startIndex + 1] + _sides);
                Mesh.Indices[startIndex + 5] = Convert.ToUInt32(Mesh.Indices[startIndex] + _sides);

                for (uint s = 1; s < _sides - 1; ++s)
                {
                    startIndex = t + (s * 6);
                    Mesh.Indices[startIndex] = Mesh.Indices[startIndex - 5];
                    Mesh.Indices[startIndex + 1] = Mesh.Indices[startIndex] + 1;
                    Mesh.Indices[startIndex + 2] = Mesh.Indices[startIndex - 2];
                    Mesh.Indices[startIndex + 3] = Mesh.Indices[startIndex + 1];
                    Mesh.Indices[startIndex + 4] = Mesh.Indices[startIndex + 2] + 1;
                    Mesh.Indices[startIndex + 5] = Mesh.Indices[startIndex + 2];
                }

                startIndex = Convert.ToUInt32(t + ((_sides - 1) * 6));
                Mesh.Indices[startIndex] = Mesh.Indices[startIndex - 5];
                Mesh.Indices[startIndex + 1] = Mesh.Indices[t];
                Mesh.Indices[startIndex + 2] = Mesh.Indices[startIndex - 2];
                Mesh.Indices[startIndex + 3] = Mesh.Indices[startIndex + 1];
                Mesh.Indices[startIndex + 4] = Convert.ToUInt32(Mesh.Indices[t] + _sides);
                Mesh.Indices[startIndex + 5] = Mesh.Indices[startIndex + 2];
            }
        }

        public void Draw(Device device, ICamera camera)
        {
            if(_refreshBuffers)
            {
                Mesh.CreateVertexBuffer(device);
                Mesh.CreateIndexBuffer(device);

                WireMesh.CreateVertexBuffer(device);
                WireMesh.CreateIndexBuffer(device);
            }

            if (Render)
            {
                Material.SetWorld(WorldMatrix);
                Material.SetWorldViewProjection(WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix);
                Material.SetLightDirection(LightDirection);

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
            else
            {
                WireMaterial.SetWorldViewProjection(WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix);

                device.InputAssembler.InputLayout = WireMaterial.InputLayout;
                device.InputAssembler.PrimitiveTopology = WireMesh.PrimitiveTopology;

                device.InputAssembler.SetIndexBuffer(WireMesh.IndexBuffer, Format.R32_UInt, 0);
                device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(WireMesh.VertexBuffer, WireMesh.VertexStride, 0));

                for (int i = 0; i < WireMaterial.Technique.Description.PassCount; ++i)
                {
                    WireMaterial.Technique.GetPassByIndex(i).Apply();
                    device.DrawIndexed(WireMesh.IndexCount, 0, 0);
                }
            }
        }

        public bool Intersects(Ray ray, out Vector3 intersectionPoint)
        {
            float distance = float.MaxValue;
            intersectionPoint = Vector3.Zero;
            bool hit = false;
            Matrix modelInverse = Matrix.Invert(WorldMatrix);

            Ray r = new Ray(ray.Position, ray.Direction);
            Vector3.Transform(r.Position, modelInverse);
            Vector3.TransformNormal(r.Direction, modelInverse);
            r.Direction.Normalize();

            for (int i = 0; i < Mesh.IndexCount; i += 3)
            {
                uint t1 = Mesh.Indices[i];
                uint t2 = Mesh.Indices[i + 1];
                uint t3 = Mesh.Indices[i + 2];

                var v = Vector3.Zero;

                Vector3 p1; Vector3 p2; Vector3 p3;
                Vector3.Transform(ref Mesh.Vertices[t1].Position, ref _worldMatrix, out p1);
                Vector3.Transform(ref Mesh.Vertices[t2].Position, ref _worldMatrix, out p2);
                Vector3.Transform(ref Mesh.Vertices[t3].Position, ref _worldMatrix, out p3);

                if (ray.Intersects(ref p1, ref p2, ref p3, out v))
                {
                    float dist = Vector3.Distance(ray.Position, v);
                    if (dist < distance)
                    {
                        intersectionPoint = v;
                        distance = dist;
                        hit = true;
                    }
                }
            }

            return hit;
        }

        public void ResetCollisionFlags()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
