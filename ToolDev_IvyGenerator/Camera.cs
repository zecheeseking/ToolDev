using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DirectInput;


namespace ToolDev_IvyGenerator
{
    public class Camera
    {

        private float _camMoveSpeed = 5.0f;

        private Vector3 _position = new Vector3(0.0f);
        public Vector3 Position { get { return _position;} }
        private Quaternion _rotation = Quaternion.Identity;

        private float _TotalYaw = 0.0f;
        private float _TotalPitch = 0.0f;

        private readonly Keyboard _kb;
        private readonly Mouse _mouse;

        public Matrix ProjectionMatrix { get; }
        private Matrix _viewMatrix;
        public Matrix ViewMatrix { get { return _viewMatrix; } }

        private Matrix _transformationMatrix = Matrix.Identity;

        public Camera(float width, float height)
        {
            ProjectionMatrix = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, width / height, 0.1f, 1000f);

            _viewMatrix = Matrix.LookAtLH(_position + Vector3.ForwardLH, Vector3.Zero, Vector3.UnitY);

            var di = new DirectInput();
            _kb = new Keyboard(di);
            _kb.Acquire();
            _mouse = new Mouse(di);
            _mouse.Acquire();
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
            UpdateTransformationMatrix();
        }

        public void Update(float deltaT)
        {
            UpdateTransformationMatrix();

            var mouseState = _mouse.GetCurrentState();
            Vector2 mouseDelta = new Vector2(mouseState.X, mouseState.Y);
            _TotalYaw += mouseDelta.X*5.0f*deltaT;
            _TotalPitch += mouseDelta.Y*5.0f*deltaT;

            _rotation = Quaternion.RotationYawPitchRoll((float)MathHelper.AngleToRadians(_TotalYaw), 
                (float)MathHelper.AngleToRadians(_TotalPitch), 
                0);

            var kbState = _kb.GetCurrentState();
            if (kbState.IsPressed(Key.W))
            {
                var dir = GetViewForward();
                _position += dir * _camMoveSpeed * deltaT;
            }
            else if (kbState.IsPressed(Key.S))
            {
                var dir = GetViewForward() * -1;
                _position += dir * _camMoveSpeed * deltaT;
            }

            if (kbState.IsPressed(Key.A))
            {
                var dir = GetViewRight() * -1;
                _position += dir * _camMoveSpeed * deltaT;
            }
            else if (kbState.IsPressed(Key.D))
            {
                var dir = GetViewRight();
                _position += dir * _camMoveSpeed * deltaT;
            }
        }

        private void UpdateTransformationMatrix()
        {
            _transformationMatrix = Matrix.Scaling(1.0f) * Matrix.RotationQuaternion(_rotation) * Matrix.Translation(_position);

            var camForward = Vector3.Transform(Vector3.ForwardLH, _rotation);
            var camRight = Vector3.Transform(Vector3.Right, _rotation);
            var camUp = Vector3.Cross(camForward, camRight);

            _viewMatrix = Matrix.LookAtLH(_position, _position + camForward, camUp);
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