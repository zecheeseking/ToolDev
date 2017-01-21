using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using ToolDev_IvyGenerator.DirectX;
using Device = SharpDX.Direct3D11.Device;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator.Interfaces
{
    public interface IIntersect
    {
        bool Selected { get; set; }
        bool Intersects(Ray ray, out Vector3 intersectionPoint);
        void ResetCollisionFlags();
    }

    public interface IMesh
    {
        Vector3 LightDirection { get; set; }
        IEffect Material { get; set; }
        MeshData<VertexPosColNorm> Mesh { get; set; }
    }

    public interface ISceneObject : ITransform
    {
        void Initialize(Device device);
        void Update(float deltaT);
        void Draw(AppContext appContext);
    }

    public interface ITransform
    {
        Matrix WorldMatrix { get; set; }

        Vec3 Position { get; set; }
        Vec3 Rotation { get; set; }
        Vec3 Scale { get; set; }
    }

    public interface IEffect
    {
        EffectTechnique Technique { get; set; }
        Effect Effect { get; set; }
        InputLayout InputLayout { get; set; }

        void Create(Device device);
        void SetWorld(Matrix world);
        void SetWorldViewProjection(Matrix wvp);
        void SetLightDirection(Vector3 dir);
    }

    public interface IModel<T>
    {
        PrimitiveTopology PrimitiveTopology { get; set; }
        int VertexStride { get; set; }
        int IndexCount { get; set; }
        T[] Vertices { get; set; }
        uint[] Indices { get; set; }
        Buffer IndexBuffer { get; set; }
        Buffer VertexBuffer { get; set; }
        IEffect Material { get; set; }
        void CreateVertexBuffer(Device device);
        void CreateIndexBuffer(Device device);
        void Draw(Device device, ICamera camera, Vector3 lightDirection);
    }
}
