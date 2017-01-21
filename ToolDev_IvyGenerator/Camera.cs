using ToolDev_IvyGenerator.Interfaces;
using SharpDX;
using ToolDev_IvyGenerator.DirectX;
using ToolDev_IvyGenerator.Utilities;
using Key = SharpDX.DirectInput.Key;


namespace ToolDev_IvyGenerator
{
    public class Camera : ICamera, ITransform
    {
        private float _camMoveSpeed = 25.0f;

        public Vec3 Position { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; }

        private float _TotalYaw = 0.0f;
        private float _TotalPitch = 0.0f;

        private Vector2 _mouseMovement;

        private Matrix _projectionMatrix;
        public Matrix ProjectionMatrix { get { return _projectionMatrix; } }
        private Matrix _viewMatrix;
        public Matrix ViewMatrix { get { return _viewMatrix; } }
        private Matrix _transformationMatrix = Matrix.Identity;
        public Matrix TransformationMatrix { get { return _transformationMatrix; }}

        private Vector3 _camForward;
        public Vector3 CameraForward { get { return _camForward; } }
        private Vector3 _camRight;
        public Vector3 CameraRight { get { return _camRight; } }

        private float _screenWidth;
        private float _screenHeight;
        public Matrix WorldMatrix { get; set; }

        public bool MovementEnabled { get; set; }
        public Camera()
        {
            Position = new Vec3 {Value = Vector3.Zero};
            Rotation = new Vec3 {Value = Vector3.Zero};
            Scale = new Vec3 {Value = new Vector3(1.0f)};
        }

        public void Initialize(float width, float height)
        {
            SetScreenWidthHeight(width, height);
            Position.Value = new Vector3(75, 75, -100);
            UpdateTransformationMatrix();

            _viewMatrix = Matrix.LookAtLH(Position.Value + Vector3.ForwardLH, Vector3.Zero, Vector3.UnitY);
        }

        public void SetScreenWidthHeight(float width, float height)
        {
            _screenWidth = width;
            _screenHeight = height;

            _projectionMatrix = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, _screenWidth / _screenHeight, 0.1f, 1000f);
        }

        public void Update(float deltaT)
        {
            _mouseMovement = InputManager.Instance.GetMouseDelta();

            if (MovementEnabled)
            {
                _TotalYaw += _mouseMovement.X * 5.0f * deltaT;
                _TotalPitch += _mouseMovement.Y * 5.0f * deltaT;

                Rotation.Value = new Vector3(_TotalPitch, _TotalYaw, 0);

                UpdateTransformationMatrix();

                var kbState = InputManager.Instance.GetCurrenKeyboardState();
                if (kbState.IsPressed(Key.W))
                {
                    var dir = GetViewForward();
                    Position.Value += dir * _camMoveSpeed * deltaT;
                }
                else if (kbState.IsPressed(Key.S))
                {
                    var dir = GetViewForward() * -1;
                    Position.Value += dir * _camMoveSpeed * deltaT;
                }

                if (kbState.IsPressed(Key.A))
                {
                    var dir = GetViewRight() * -1;
                    Position.Value += dir * _camMoveSpeed * deltaT;
                }
                else if (kbState.IsPressed(Key.D))
                {
                    var dir = GetViewRight();
                    Position.Value += dir * _camMoveSpeed * deltaT;
                }
            }
        }

        private void UpdateTransformationMatrix()
        {
            _transformationMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);

            Quaternion q = Quaternion.RotationMatrix(_transformationMatrix);

            _camForward = Vector3.Transform(Vector3.ForwardLH, q);
            _camRight = Vector3.Transform(Vector3.Right, q);
            var camUp = Vector3.Cross(_camForward, _camRight);

            _viewMatrix = Matrix.LookAtLH(Position.Value, Position.Value + _camForward, camUp);
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
            v.X = (((2.0f * mousePosX) / _screenWidth) -1.0f) / _projectionMatrix.M11;
            v.Y = -(((2.0f * mousePosY) / _screenHeight) - 1.0f)/ _projectionMatrix.M22;
            v.Z = 1.0f;

            Vector3 rPos = Vector3.Zero;
            Vector3 rDir = Vector3.Zero;

            Matrix inverseView = Matrix.Zero;
            Matrix.Invert(ref _viewMatrix, out inverseView);

            rDir.X = (v.X * inverseView.M11) + (v.Y * inverseView.M21) + (v.Z * inverseView.M31);
            rDir.Y = (v.X * inverseView.M12) + (v.Y * inverseView.M22) + (v.Z * inverseView.M32);
            rDir.Z = (v.X * inverseView.M13) + (v.Y * inverseView.M23) + (v.Z * inverseView.M33);
            rPos.X = inverseView.M41;
            rPos.Y = inverseView.M42;
            rPos.Z = inverseView.M43;

            return new Ray(rPos, rDir);
        }
    }
}