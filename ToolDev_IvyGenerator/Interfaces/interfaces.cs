using System;
using SharpDX;

namespace ToolDev_IvyGenerator.Interfaces
{
    public interface ITransform
    {
        Matrix WorldMatrix { get; }
        Vector3 Position { get; }
        Quaternion Rotation { get; }
        Vector3 Scale { get; }
        void Translate(float x, float y, float z);
        void Rotate();
        void Scaling(float x, float y, float z);
    }
}
