using System;
using SharpDX;

namespace ToolDev_IvyGenerator.Interfaces
{
    public interface ITransform
    {
        Matrix WorldMatrix { get; set; }
        void Translate(float x, float y, float z);
        void Rotate();
        void Scale();
    }
}
