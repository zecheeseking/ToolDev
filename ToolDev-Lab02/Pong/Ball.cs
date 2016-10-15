using System;
using System.Windows;
using System.Windows.Media;

namespace Pong
{
    internal class Ball
    {
        private Point _pos;
        private Point _startPos;

        public Brush Color { get; set; }
        public float SpeedX { get; set; }
        public float SpeedY { get; set; }
        public Size BallDimensions { get; set; }
        public Size ScreenDimensions { get; set; }
        public Rect HitRect { get; set; }

        public event Action<bool> BallOut;

        public Ball(Point position)
        {
            _pos = _startPos = position;
            Color = Brushes.Blue;
            SpeedX = SpeedY = 150;
            BallDimensions = new Size(20, 20);
            ScreenDimensions = new Size(800, 600);
        }

        public void HitTest(Paddle paddle)
        {
            var intersection = Rect.Intersect(HitRect, paddle.HitRect);

            if (intersection.IsEmpty)
                return;

            SpeedX *= -1;

            var newX = intersection.X - BallDimensions.Width;
            if (intersection.X > paddle.Position.X)
                newX = intersection.X + intersection.Width + BallDimensions.Width;

            _pos.X = newX;
        }

        public void Update(float deltaT)
        {
            if (_pos.X < 0)
            {
                if (BallOut != null)
                    BallOut(true);
            }

            if (_pos.X > ScreenDimensions.Width)
            {
                if (BallOut != null)
                    BallOut(false);
            }

            if (_pos.Y - BallDimensions.Height < 0)
            {
                _pos.Y = BallDimensions.Height;
                SpeedY *= -1;
            }

            if (_pos.Y + BallDimensions.Height > ScreenDimensions.Height)
            {
                _pos.Y = ScreenDimensions.Height - BallDimensions.Height;
                SpeedY *= -1;
            }

            _pos.X += SpeedX * deltaT;
            _pos.Y += SpeedY * deltaT;

            HitRect = new Rect(_pos, BallDimensions);
        }

        public void Reset()
        {
            _pos = _startPos;
        }

        public void Draw(DrawingContext dc)
        {
            dc.DrawEllipse(Color, null, _pos, BallDimensions.Width, BallDimensions.Height);
        }
    }
}