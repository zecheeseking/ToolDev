using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using System.Runtime.InteropServices;
using ToolDev_IvyGenerator.Effects;
using ToolDev_IvyGenerator.Interfaces;
using ToolDev_IvyGenerator.Utilities;
using Device = SharpDX.Direct3D10.Device1;

namespace ToolDev_IvyGenerator.DirectX
{
    public class Line : ISceneObject
    {
        public Vector3 LightDirection { get; set; }

        public IEffect Material { get; set; }

        public MeshData<VertexPosColNorm> Mesh { get; set; }

        public Vec3 Position { get; set; }

        public Vec3 Rotation { get; set; }

        public Vec3 Scale { get; set; }

        public Matrix WorldMatrix { get; set; }

        private Vec3[] _linePositions = new Vec3[2];
        public Vec3 StartPosition
        {
            get { return _linePositions[0]; }
            set
            {
                _linePositions[0] = value;
                _updateBuffers = true;
            }
        }

        public Vec3 EndPosition
        {
            get { return _linePositions[1]; }
            set
            {
                _linePositions[1] = value;
                _updateBuffers = true;
            }
        }

        private bool _updateBuffers = false;

        public void Initialize(Device device)
        {
            WorldMatrix = Matrix.Scaling(1.0f) * Matrix.RotationQuaternion(Quaternion.Identity) * Matrix.Translation(Vector3.Zero);

            Mesh = new MeshData<VertexPosColNorm>();
            Mesh.PrimitiveTopology = PrimitiveTopology.LineList;

            Mesh.Vertices = new VertexPosColNorm[2];
            Mesh.Vertices[0] = new VertexPosColNorm(_linePositions[0].Value, Color.Gray);
            Mesh.Vertices[1] = new VertexPosColNorm(_linePositions[1].Value, Color.Gray);
            Mesh.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));

            Mesh.Indices = new uint[2];
            Mesh.Indices[0] = 0;
            Mesh.Indices[1] = 1;
            Mesh.IndexCount = 2;

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
            if(_updateBuffers)
            {
                Mesh.Vertices[0] = new VertexPosColNorm(_linePositions[0].Value, Color.Gray);
                Mesh.Vertices[1] = new VertexPosColNorm(_linePositions[1].Value, Color.Gray);

                Mesh.CreateVertexBuffer(device);

                _updateBuffers = false;
            }

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
    }
}
