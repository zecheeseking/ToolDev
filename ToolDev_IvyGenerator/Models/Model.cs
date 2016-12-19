﻿
using SharpDX.Direct3D;
using DaeSharpWpf;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using DaeSharpWPF;
using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using ToolDev_IvyGenerator.Annotations;
using ToolDev_IvyGenerator.Effects;
using ToolDev_IvyGenerator.Interfaces;
using Device = SharpDX.Direct3D10.Device1;

namespace ToolDev_IvyGenerator.Models
{

    public class Model : IModel<VertexPosColNorm>, INotifyPropertyChanged, ITransform
    {
        public PrimitiveTopology PrimitiveTopology { get; set; }
        public int VertexStride { get; set; }
        public int IndexCount { get; set; }
        public VertexPosColNorm[] Vertices { get; set; }
        public uint[] Indices { get; set; }
        public SharpDX.Direct3D10.Buffer IndexBuffer { get; set; }
        public SharpDX.Direct3D10.Buffer VertexBuffer { get; set; }
        public IEffect Material { get; set; }

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
            Material.SetWorld(WorldMatrix);
            Material.SetWorldViewProjection(WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix);
            Material.SetLightDirection(lightDirection);

            device.InputAssembler.InputLayout = Material.InputLayout;
            device.InputAssembler.PrimitiveTopology = PrimitiveTopology;
            device.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, VertexStride, 0));

            for (int i = 0; i < Material.Technique.Description.PassCount; ++i)
            {
                Material.Technique.GetPassByIndex(i).Apply();
                device.DrawIndexed(IndexCount, 0, 0);
            }
        }

        private Matrix _worldMatrix;

        public Matrix WorldMatrix{ get { return _worldMatrix; } }

        private Vector3 _position;

        public Vector3 Position { get { return _position; } }

        private Quaternion _rotation;
        public Quaternion Rotation { get { return _rotation; } }
        private Vector3 _scale;
        public Vector3 Scale { get {return _scale; } }

        public void Translate(float x, float y, float z)
        {
            _position = new Vector3(x,y,z);

            UpdateWorldMatrix();
        }

        public void Rotate()
        {

        }

        public void Scaling(float x, float y, float z)
        {
            _scale = new Vector3(x, y, z);

            UpdateWorldMatrix();
        }

        private void UpdateWorldMatrix()
        {
            _worldMatrix = Matrix.Scaling(Scale) * Matrix.RotationQuaternion(Rotation) * Matrix.Translation(Position);
        }

        public Model()
        {
            _position = Vector3.Zero;
            _rotation = Quaternion.Identity;
            _scale = new Vector3(1.0f);

            UpdateWorldMatrix();
        }

        public Model(Model model)
        {
            _position = model._position;
            _rotation = model._rotation;
            _scale = model._scale;
            UpdateWorldMatrix();

            PrimitiveTopology = model.PrimitiveTopology;
            Material = model.Material;
            Vertices = model.Vertices;
            Indices = model.Indices;
            IndexCount = model.IndexCount;
            VertexStride = model.VertexStride;
            VertexBuffer = model.VertexBuffer;
            IndexBuffer = model.IndexBuffer;
        }

        public bool Intersects(Ray ray, out Vector3 intersectionPoint)
        {
            float distance = float.MaxValue;
            intersectionPoint = Vector3.Zero;
            bool hit = false;

            for (int i = 0; i < IndexCount; i += 3)
            {
                uint t1 = Indices[i];
                uint t2 = Indices[i + 1];
                uint t3 = Indices[i + 2];

                var v = Vector3.Zero;

                if (ray.Intersects(ref Vertices[t1].Position, ref Vertices[t2].Position, ref Vertices[t3].Position, out v))
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}