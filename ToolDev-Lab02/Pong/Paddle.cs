using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pong
{
    internal class Paddle
    {
        private Point _pos;
        private Point _startPos;

        public Point Position { get { return _pos; } }
        public Brush Color { get; set; }
        public Rect HitRect { get; set; }
        public Size PaddleDimensions { get; set; }
        public Size ScreenDimensions { get; set; }
        public Key UpKey { get; set; }
        public Key DownKey { get; set; }
        public float Speed { get; set; }

        public Paddle(Point position)
        {
            _pos = _startPos = position;
            Speed = 250.0f;
            PaddleDimensions = new Size(20, 100);
            ScreenDimensions = new Size(800, 600);
            HitRect = new Rect(_pos, PaddleDimensions);
        }

        public void Update(float deltaT)
        {
            var upState = Keyboard.GetKeyStates(UpKey);
            if (upState.HasFlag(KeyStates.Down))
            {
                _pos.Y -= Speed * deltaT;
            }

            var downState = Keyboard.GetKeyStates(DownKey);
            if (downState.HasFlag(KeyStates.Down))
            {
                _pos.Y += Speed * deltaT;
            }

            if (_pos.Y < 0)
                _pos.Y = 0;

            if (_pos.Y + PaddleDimensions.Height > ScreenDimensions.Height)
                _pos.Y = ScreenDimensions.Height - PaddleDimensions.Height;

            HitRect = new Rect(_pos, PaddleDimensions);
        }

        public void Reset()
        {
            _pos = _startPos;
        }

        public void Draw(DrawingContext dc)
        {
            dc.DrawRectangle(Color, null, HitRect);
        }
    }
}