
using System;
using System.Runtime.InteropServices;
using ToolDev_IvyGenerator.Interfaces;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using ToolDev_IvyGenerator.Effects;
using Buffer = SharpDX.Direct3D10.Buffer;
using Device = SharpDX.Direct3D10.Device1;
using ToolDev_IvyGenerator.DirectX;

namespace ToolDev_IvyGenerator.Utilities
{
    public class SceneGrid : ISceneObject
    {
        public Matrix WorldMatrix { get; set; }
        public Vec3 Position { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; }

        public IEffect Material { get; set; }
        public Vector3 LightDirection { get; set; }
        public MeshData<VertexPosColNorm> Mesh { get; set; }


        public void Initialize(Device device)
        {
            WorldMatrix = Matrix.Scaling(1.0f)*Matrix.RotationQuaternion(Quaternion.Identity)*Matrix.Translation(Vector3.Zero);

            Mesh = new MeshData<VertexPosColNorm>();
            Mesh.PrimitiveTopology = PrimitiveTopology.LineList;

            CreateGrid(20, Color.Black, 4.0f);

            Mesh.CreateVertexBuffer(device);
            Mesh.CreateIndexBuffer(device);

            Material = new SceneGridEffect();
            Material.Create(device);
        }

        public void Update(float deltaT)
        {
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

        private void CreateGrid(int gridSize, Color color, float gridSpacing)
        {
            float zeroOffset = ((float)gridSize / 2) * gridSpacing;

            Mesh.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));
            Mesh.Vertices = new VertexPosColNorm[((gridSize + 1) * 2) * 2];
            Mesh.Indices = new uint[((gridSize + 1) * 2) * 2];

            for (int i = 0; i < Mesh.Vertices.Length / 2; i += 2)
            {
                //Horizontal
                Mesh.Vertices[i] = new VertexPosColNorm(new Vector3(0 - zeroOffset,0,0 - zeroOffset + i * (gridSpacing / 2)), color, Vector3.Right);
                Mesh.Vertices[i + 1] = new VertexPosColNorm(new Vector3(0 + zeroOffset,0,0 - zeroOffset + i * (gridSpacing / 2)), color, Vector3.Right);
                Mesh.Indices[i] = Convert.ToUInt32(i);
                Mesh.Indices[i + 1] = Convert.ToUInt32(i + 1);
            }

            //vertical
            for (int i = Mesh.Vertices.Length / 2; i < Mesh.Vertices.Length; i += 2)
            {
                Mesh.Vertices[i] = new VertexPosColNorm(new Vector3(0 - zeroOffset + (i - (Mesh.Vertices.Length / 2)) * (gridSpacing / 2), 0,0 - zeroOffset), color, Vector3.Right);
                Mesh.Vertices[i + 1] = new VertexPosColNorm(new Vector3(0 - zeroOffset + (i - (Mesh.Vertices.Length / 2)) * (gridSpacing / 2), 0,0 + zeroOffset), color, Vector3.Right);
                Mesh.Indices[i] = Convert.ToUInt32(i);
                Mesh.Indices[i + 1] = Convert.ToUInt32(i + 1);
            }

            Mesh.IndexCount = Mesh.Indices.Length;
        }
    }
}