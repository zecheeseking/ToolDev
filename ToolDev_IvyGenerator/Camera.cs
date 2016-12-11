using SharpDX;
using SharpDX.Direct3D10;

namespace ToolDev_IvyGenerator
{
    public class Camera
    {

        private Vector3 _position;
        public Vector3 Position { get { return _position;} }
        private Quaternion _rotation;

        private Matrix _projectionMat;
        public Matrix ProjectionMatrix { get { return _projectionMat; } }

        public Camera(float width, float height)
        {
            _position = new Vector3(0.0f);
            _rotation = Quaternion.Identity;
            _projectionMat = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, width / height, 0.1f, 1000f);
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }
    }
}