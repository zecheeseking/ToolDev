using SharpDX;

namespace ToolDev_IvyGenerator.Utilities
{
    public class Vec3
    {
        private Vector3 _vector;

        public Vec3()
        {
            _vector = Vector3.Zero;
        }

        public Vector3 Value
        {
            get { return _vector; }
            set { _vector = value; }
        }

        public float X
        {
            get { return _vector.X; }
            set { _vector.X = value; }    
        }

        public float Y
        {
            get { return _vector.Y; }
            set { _vector.Y = value; }
        }

        public float Z
        {
            get { return _vector.Z; }
            set { _vector.Z = value; }
        }
    }
}
