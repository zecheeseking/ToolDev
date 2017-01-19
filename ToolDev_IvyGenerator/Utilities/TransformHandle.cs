using System.Diagnostics;
using System.Runtime.InteropServices;
using ToolDev_IvyGenerator.Interfaces;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using ToolDev_IvyGenerator.Effects;
using Device1 = SharpDX.Direct3D10.Device1;

namespace ToolDev_IvyGenerator.Utilities
{
    public class TransformHandle : ISceneObject, IIntersect
    {
        public Matrix WorldMatrix { get; set; }
        private Vec3 _position;
        public bool Selected { get; set; }
        public Vec3 Position
        {
            get {return _position;}
            set
            {
                _position = value;

                _xBoundingBox.Minimum = new Vector3(0.0f, -1.0f, -1.0f) + _position.Value;
                _xBoundingBox.Maximum = new Vector3(10.0f, 1.0f, 1.0f) + _position.Value;
                _yBoundingBox.Minimum = new Vector3(-1.0f, 0.0f, -1.0f) + _position.Value;
                _yBoundingBox.Maximum = new Vector3(1.0f, 10.0f, 1.0f) + _position.Value;
                _zBoundingBox.Minimum = new Vector3(-1.0f, -1.0f, 0.0f) + _position.Value;
                _zBoundingBox.Maximum = new Vector3(1.0f, 1.0f, 10.0f) + _position.Value;
            }
        }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; }

        public IEffect Material { get; set; }
        public Vector3 LightDirection { get; set; }
        public MeshData<VertexPosColNorm> Mesh { get; set; }

        private bool _xHit = false;
        private BoundingBox _xBoundingBox;
        private bool _yHit = false;
        private BoundingBox _yBoundingBox;
        private bool _zHit = false;
        private BoundingBox _zBoundingBox;

        public void Initialize(Device1 device)
        {
            Mesh = new MeshData<VertexPosColNorm>();

            Mesh.PrimitiveTopology = PrimitiveTopology.LineList;

            CreateTransformLines();

            Mesh.CreateVertexBuffer(device);
            Mesh.CreateIndexBuffer(device);

            Material = new DebugEffect();
            Material.Create(device);

            _xBoundingBox = new BoundingBox();
            _yBoundingBox = new BoundingBox();
            _zBoundingBox = new BoundingBox();

            UpdateBoundingBoxes();
        }

        private void CreateTransformLines()
        {
            Mesh.Vertices = new VertexPosColNorm[6];
            Mesh.Vertices[0] = new VertexPosColNorm(Vector3.Zero, Color.Blue, Vector3.Up);
            Mesh.Vertices[1] = new VertexPosColNorm(Vector3.Zero, Color.Red, Vector3.Up);
            Mesh.Vertices[2] = new VertexPosColNorm(Vector3.Zero, Color.Green, Vector3.Up);
            Mesh.Vertices[3] = new VertexPosColNorm(Vector3.ForwardLH * 10.0f, Color.Blue, Vector3.Up);
            Mesh.Vertices[4] = new VertexPosColNorm(Vector3.Right * 10.0f, Color.Red, Vector3.Up);
            Mesh.Vertices[5] = new VertexPosColNorm(Vector3.Up * 10.0f, Color.Green, Vector3.Up);
            Mesh.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));

            Mesh.Indices = new uint[6];
            Mesh.Indices[0] = 0;
            Mesh.Indices[1] = 3;
            Mesh.Indices[2] = 1;
            Mesh.Indices[3] = 4;
            Mesh.Indices[4] = 2;
            Mesh.Indices[5] = 5;
            Mesh.IndexCount = Mesh.Indices.Length;
        }

        private void UpdateBoundingBoxes()
        {
            _xBoundingBox.Minimum = new Vector3(0.0f, -1.0f, -1.0f) + Position.Value;
            _xBoundingBox.Maximum = new Vector3(10.0f, 1.0f, 1.0f) + Position.Value;

            _yBoundingBox.Minimum = new Vector3(-1.0f, 0.0f, -1.0f) + Position.Value;
            _yBoundingBox.Maximum = new Vector3(1.0f, 10.0f, 1.0f) + Position.Value;

            _zBoundingBox.Minimum = new Vector3(-1.0f, -1.0f, 0.0f) + Position.Value;
            _zBoundingBox.Maximum = new Vector3(1.0f, 1.0f, 10.0f) + Position.Value;
        }

        public void Update(float deltaT)
        {
            UpdateBoundingBoxes();

            var mouseMovement = InputManager.Instance.GetMouseDelta();

            if (_xHit && InputManager.Instance.GetMouseButton(0))
            {
                Debug.WriteLine("xhit");
                Position.Value = _position.Value + (Vector3.Right * mouseMovement.X);
            }
            else if (_yHit && InputManager.Instance.GetMouseButton(0))
            {
                Debug.WriteLine("yhit");
                Position.Value = _position.Value + (Vector3.Up * -mouseMovement.Y);
            }
            else if (_zHit && InputManager.Instance.GetMouseButton(0))
            {
                Debug.WriteLine("zhit");
                Position.Value = _position.Value + (Vector3.ForwardRH * -1 * mouseMovement.X);
            }

            WorldMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);
        }

        public void Draw(Device1 device, ICamera camera)
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

        public bool Intersects(Ray ray, out Vector3 intersectionPoint)
        {
            intersectionPoint = Vector3.Zero;

            _xHit = ray.Intersects(_xBoundingBox);

            _yHit = ray.Intersects(_yBoundingBox);

            _zHit = ray.Intersects(_zBoundingBox);

            return (_xHit | _yHit | _zHit);
        }

        public void ResetCollisionFlags()
        {
            _xHit = false;
            _yHit = false;
            _zHit = false;
        }
    }
}
