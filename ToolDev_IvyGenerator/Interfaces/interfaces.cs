using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using Device = SharpDX.Direct3D10.Device1;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator.Interfaces
{
    public interface IIntersect
    {
        bool Selected { get; set; }
        bool Intersects(Ray ray, out Vector3 intersectionPoint);
        void ResetCollisionFlags();
    }

    public interface ISceneObject : ITransform
    {
        IEffect Material { get; set; }
        Vector3 LightDirection { get; set; }
        MeshData<VertexPosColNorm> Mesh { get; set; }


        void Initialize(Device device);
        void Update(float deltaT);
        void Draw(Device device, ICamera camera);
    }

    public interface ITransform
    {
        Matrix WorldMatrix { get; set; }

        Vec3 Position { get; set; }
        Vec3 Rotation { get; set; }
        Vec3 Scale { get; set; }
    }

    public interface ICamera
    {
        Matrix TransformationMatrix { get; }
        Matrix ViewMatrix { get; }
        Matrix ProjectionMatrix { get; }

        void Initialize(float width, float height);
        void Update(float deltaT);
    }

    public interface IEffect
    {
        EffectTechnique Technique { get; set; }
        Effect Effect { get; set; }
        InputLayout InputLayout { get; set; }

        void Create(Device1 device);
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
