using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToolDev_IvyGenerator.Interfaces;
using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using ToolDev_IvyGenerator.Annotations;
using ToolDev_IvyGenerator.Effects;
using Device = SharpDX.Direct3D10.Device1;
using ToolDev_IvyGenerator.Utilities;
using ToolDev_IvyGenerator.DirectX;

using System.Diagnostics;

namespace ToolDev_IvyGenerator.Models
{
    public class Model : ISceneObject, INotifyPropertyChanged, IIntersect
    {
        public SharpDX.Vector3 LightDirection { get; set; }
        public TransformComponent Transform { get; set; }
        public IEffect Material { get; set; }
        public MeshData<VertexPosColNorm> Mesh { get; set; }
        public bool Selected { get; set; }
        public Color Color { get; set; }

        public Model()
        {
            Material = new PosNormColEffect();

            Color = Color.Gray;

            LightDirection = Vector3.Zero;
            Transform = new TransformComponent();
        }

        public Model(Model model)
        {
            Material = model.Material;
            Mesh = model.Mesh;

            LightDirection = model.LightDirection;
        }

        public void Initialize(Device device)
        {
            Material.Create(device);

            for (int i = 0; i < Mesh.Positions.Length; ++i)
                Mesh.Vertices[i] = new VertexPosColNorm(Mesh.Positions[i], Mesh.Colors[i], Mesh.Normals[i]);

            Mesh.CreateVertexBuffer(device);
            Mesh.CreateIndexBuffer(device);

            Transform.Initialize(device);
        }

        public void Update(float deltaT)
        {
            Transform.Update(deltaT);
        }

        public void Draw(Device device, ICamera camera)
        {
            Material.SetWorld(Transform.WorldMatrix);
            Material.SetWorldViewProjection(Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix);
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

            if (Selected)
                Transform.Draw(device, camera);
        }

        public bool Intersects(Ray ray, out Vector3 intersectionPoint)
        {
            float distance = float.MaxValue;
            intersectionPoint = Vector3.Zero;
            bool hit = false;

            if (Selected)
                return Transform.Intersects(ray, out intersectionPoint);

            Matrix modelInverse = Matrix.Invert(Transform.WorldMatrix);

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
                Vector3.Transform(ref Mesh.Vertices[t1].Position, ref Transform.WorldMatrix, out p1);
                Vector3.Transform(ref Mesh.Vertices[t2].Position, ref Transform.WorldMatrix, out p2);
                Vector3.Transform(ref Mesh.Vertices[t3].Position, ref Transform.WorldMatrix, out p3);

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
            Transform.ResetCollisionFlags();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
