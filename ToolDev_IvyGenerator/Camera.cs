using DaeSharpWpf;
using DaeSharpWPF;
using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DirectInput;
using ToolDev_IvyGenerator.Interfaces;


namespace ToolDev_IvyGenerator
{
    public class Camera : ICamera, ITransform
    {
        private float _camMoveSpeed = 5.0f;

        private Vector3 _position = new Vector3(0.0f);
        public Vector3 Position { get { return _position;} }
        private Quaternion _rotation = Quaternion.Identity;

        private float _TotalYaw = 0.0f;
        private float _TotalPitch = 0.0f;

        private readonly Keyboard _kb;
        private readonly Mouse _mouse;

        private Matrix _projectionMatrix;
        public Matrix ProjectionMatrix { get { return _projectionMatrix; } }
        private Matrix _viewMatrix;
        public Matrix ViewMatrix { get { return _viewMatrix; } }
        private Matrix _transformationMatrix = Matrix.Identity;
        public Matrix TransformationMatrix { get { return _transformationMatrix; }}

        private float _screenWidth;
        private float _screenHeight;
        public Matrix WorldMatrix { get; set; }

        public bool MovementEnabled { get; set; }
        public Camera()
        {
            var di = new DirectInput();
            _kb = new Keyboard(di);
            _kb.Acquire();
            _mouse = new Mouse(di);
            _mouse.Acquire();
        }

        public void Initialize(float width, float height)
        {
            SetScreenWidthHeight(width, height);
            _position = new Vector3(50, 50, -100);
            UpdateTransformationMatrix();

            _viewMatrix = Matrix.LookAtLH(_position + Vector3.ForwardLH, Vector3.Zero, Vector3.UnitY);

        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
            UpdateTransformationMatrix();
        }

        public void SetScreenWidthHeight(float width, float height)
        {
            _screenWidth = width;
            _screenHeight = height;

            _projectionMatrix = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, _screenWidth / _screenHeight, 0.1f, 1000f);
        }

        public void Update(float deltaT)
        {
            if (MovementEnabled)
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
            return new Vector3(ViewMatrix.M13, ViewMatrix.M23, ViewMatrix.M33);
        }

        private Vector3 GetViewRight()
        {
            return new Vector3(ViewMatrix.M11, ViewMatrix.M21, ViewMatrix.M31);
        }

        public Ray GetPickRay(float mousePosX, float mousePosY)
        {
            Vector3 v = Vector3.Zero;

            v.X = (((2.0f * mousePosX) / _screenWidth) - 1.0f) / _projectionMatrix.M11;
            v.Y = (((2.0f * mousePosY) / _screenHeight) - 1.0f) / _projectionMatrix.M22;
            v.Z = 1.0f;

            Vector3 rPos = Vector3.Zero;
            Vector3 rDir = Vector3.Zero;

            Matrix inverseView = _viewMatrix;
            inverseView.Invert();

            rDir.X = v.X * inverseView.M11 + v.Y * inverseView.M21 + v.Z * inverseView.M31;
            rDir.Y = v.X * inverseView.M12 + v.Y * inverseView.M22 + v.Z * inverseView.M32;
            rDir.Z = v.X * inverseView.M13 + v.Y * inverseView.M23 + v.Z * inverseView.M33;
            rPos.X = inverseView.M41;
            rPos.Y = inverseView.M42;
            rPos.Z = inverseView.M43;

            return new Ray(rPos, rDir);
        }

        public void Translate(float x, float y, float z)
        {
        }

        public void Rotate()
        {
        }

        public void Scale()
        {
        }
    }
}