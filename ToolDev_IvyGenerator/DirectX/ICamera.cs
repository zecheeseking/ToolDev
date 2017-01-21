using SharpDX;

namespace ToolDev_IvyGenerator.DirectX
{
    public interface ICamera
    {
        Matrix TransformationMatrix { get; }
        Matrix ViewMatrix { get; }
        Matrix ProjectionMatrix { get; }

        void Initialize(float width, float height);
        void Update(float deltaT);
    }
}
