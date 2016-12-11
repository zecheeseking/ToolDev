using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DirectInput;

using System.Diagnostics;

namespace ToolDev_IvyGenerator
{
    public class Camera
    {

        private Vector3 _position;
        public Vector3 Position { get { return _position;} }
        private Quaternion _rotation;

        private DirectInput di;
        private Keyboard kb;

        public Matrix ProjectionMatrix { get; }

        public Camera(float width, float height)
        {
            _position = new Vector3(0.0f);
            _rotation = Quaternion.Identity;
            ProjectionMatrix = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, width / height, 0.1f, 1000f);

            di = new DirectInput();
            kb = new Keyboard(di);
            kb.Acquire();
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        public void Update(float deltaT)
        {
            var kbState = kb.GetCurrentState();

            if (kbState.IsPressed(Key.W))
                Debug.WriteLine("Forward");
            else if (kbState.IsPressed(Key.S))
                Debug.WriteLine("Backward");

            if (kbState.IsPressed(Key.A))
                Debug.WriteLine("Left");
            else if (kbState.IsPressed(Key.D))
                Debug.WriteLine("Right");
        }
    }
}