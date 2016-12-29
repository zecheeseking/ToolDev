using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToolDev_IvyGenerator.Interfaces;
using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using ToolDev_IvyGenerator.Annotations;
using ToolDev_IvyGenerator.Effects;
using Device1 = SharpDX.Direct3D10.Device1;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator.Models
{
    public class Model : ISceneObject, INotifyPropertyChanged, IIntersect
    {
        private Matrix _worldMatrix;
        public Matrix WorldMatrix { get {return _worldMatrix;} set{ _worldMatrix = value;}  }
        //public Vec3 Pos { get; set; }
        public Vec3 Position { get; set; }
        //public Vector3 Position { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; }
        public IEffect Material { get; set; }
        public MeshData<VertexPosColNorm> Mesh { get; set; }

        public SharpDX.Vector3 LightDirection { get; set; }

        public Color Color { get; set; }

        public Model()
        {
            Material = new PosNormColEffect();

            Color = Color.Gray;

            Position = new Vec3 {Value = Vector3.Zero};
            Rotation = new Vec3 {Value = Vector3.Zero};
            Scale = new Vec3 {Value = new Vector3(1.0f)};

            WorldMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);
            LightDirection = Vector3.Zero;
        }

        public Model(Model model)
        {
            Material = model.Material;

            Position = model.Position;
            Rotation = model.Rotation;
            Scale = model.Scale;
            WorldMatrix = model.WorldMatrix;

            Mesh = model.Mesh;

            LightDirection = model.LightDirection;
        }

        public void Initialize(Device1 device)
        {
            Material.Create(device);

            for (int i = 0; i < Mesh.Positions.Length; ++i)
                Mesh.Vertices[i] = new VertexPosColNorm(Mesh.Positions[i], Mesh.Colors[i], Mesh.Normals[i]);

            Mesh.CreateVertexBuffer(device);
            Mesh.CreateIndexBuffer(device);
        }

        public void Update(float deltaT)
        {
            WorldMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);
        }

        public void Draw(Device1 device, ICamera camera)
        {
            Material.SetWorld(WorldMatrix);
            Material.SetWorldViewProjection(WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix);
            Material.SetLightDirection(LightDirection);
            (Material as PosNormColEffect).SetColor(Color);

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

                Vector3 p1;Vector3 p2;Vector3 p3;
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
