using SharpDX;
using Device = SharpDX.Direct3D10.Device1;

namespace DaeSharpWpf.Interfaces
{
    public interface ISceneObject : ITransform
    {
        IEffect Material { get; set; }
        Vector3 LightDirection { get; set; }
        MeshData<VertexPosColNorm> Mesh { get; set; }

        void Initialize(Device device);
        void Update(float deltaT);
        void Draw(Device device, ICamera camera);
    }
}
