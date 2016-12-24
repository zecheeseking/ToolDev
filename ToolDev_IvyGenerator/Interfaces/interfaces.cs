using System;
using SharpDX;

namespace ToolDev_IvyGenerator.Interfaces
{
    public interface IIntersect
    {
        bool Intersects(Ray ray, out Vector3 intersectionPoint);
        void ResetCollisionFlags();
    }
}
