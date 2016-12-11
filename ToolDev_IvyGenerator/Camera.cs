using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DirectInput;

using System.Diagnostics;

namespace ToolDev_IvyGenerator
{
    public class Camera
    {

        private float _camMoveSpeed = 5.0f;

        private Vector3 _position;
        public Vector3 Position { get { return _position;} }
        private Quaternion _rotation;

        private readonly Keyboard _kb;

        public Matrix ProjectionMatrix { get; }
        private Matrix _viewMatrix;
        public Matrix ViewMatrix { get { return _viewMatrix; } }

        public Camera(float width, float height)
        {
            _position = new Vector3(0.0f);
            _rotation = Quaternion.Identity;

            ProjectionMatrix = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, width / height, 0.1f, 1000f);

            _viewMatrix = Matrix.LookAtLH(_position + Vector3.ForwardLH, Vector3.Zero, Vector3.UnitY);

            var di = new DirectInput();
            _kb = new Keyboard(di);
            _kb.Acquire();
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        public void Update(float deltaT)
        {
            _viewMatrix = Matrix.LookAtLH(_position, _position + Vector3.ForwardLH, Vector3.UnitY);

            var kbState = _kb.GetCurrentState();

            if (kbState.IsPressed(Key.W))
            {
                Debug.WriteLine("Forward");
                var forward = GetViewForward();
                _position += forward * _camMoveSpeed * deltaT;
            }
            else if (kbState.IsPressed(Key.S))
            {
                Debug.WriteLine("Backward");
                var backward = GetViewForward() * -1;
                _position += backward * _camMoveSpeed * deltaT;
            }

            if (kbState.IsPressed(Key.A))
            {
                Debug.WriteLine("Left");
                var left = GetViewRight() * -1;
                _position += left * _camMoveSpeed * deltaT;
            }
            else if (kbState.IsPressed(Key.D))
            {
                Debug.WriteLine("Right");
                var right = GetViewRight();
                _position += right * _camMoveSpeed * deltaT;
            }
        }

        private Vector3 GetViewForward()
        {
            return new Vector3(_viewMatrix.M13, _viewMatrix.M23, _viewMatrix.M33);
        }

        private Vector3 GetViewRight()
        {
            return new Vector3(_viewMatrix.M11, _viewMatrix.M21, _viewMatrix.M31);
        }
    }
}