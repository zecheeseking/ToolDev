using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pong
{
    /// <summary>
    /// Interaction logic for PongControl.xaml
    /// </summary>
    public partial class PongControl : UserControl
    {
        private DateTime _prevDrawTime;
        private const int FRAMERATE = 60;

        private Paddle _leftPaddle;
        private Paddle _rightPaddle;

        private Ball _ball;

        private int _scoreLeft;
        private int _scoreRight;

        public event Action<int, int> ScoreChanged;

        public PongControl()
        {
            InitializeComponent();

            _prevDrawTime = DateTime.Now;

            var paintTimer = new Timer
            {
                AutoReset = true,
                Interval = 1000.0 / FRAMERATE,
            };
            paintTimer.Elapsed += (o, e) => Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(InvalidateVisual));
            paintTimer.Start();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var now = DateTime.Now;
            var difference = now - _prevDrawTime;
            _prevDrawTime = now;
            var deltaT = (float)difference.TotalSeconds;

            if (deltaT > 0.3f)
                deltaT = 0.3f;

            _ball.Update(deltaT);
            _leftPaddle.Update(deltaT);
            _rightPaddle.Update(deltaT);

            _ball.HitTest(_leftPaddle);
            _ball.HitTest(_rightPaddle);

            _ball.Draw(drawingContext);
            _leftPaddle.Draw(drawingContext);
            _rightPaddle.Draw(drawingContext);

            base.OnRender(drawingContext);
        }

        private void Pong_Initialized(object sender, EventArgs e)
        {
            _leftPaddle = new Paddle(new Point(20, 250))
            {
                Color = Brushes.Green,
                UpKey = Key.W,
                DownKey = Key.S,
                ScreenDimensions = new Size(Width, Height)
            };

            _rightPaddle = new Paddle(new Point(750, 250))
            {
                Color = Brushes.Red,
                UpKey = Key.Up,
                DownKey = Key.Down,
                ScreenDimensions = new Size(Width, Height)
            };

            _ball = new Ball(new Point(400, 300))
            {
                Color = Brushes.Blue,
                ScreenDimensions = new Size(Width, Height)
            };

            _ball.BallOut += left =>
            {
                _ball.Reset();
                _leftPaddle.Reset();
                _rightPaddle.Reset();

                if (left)
                    ++_scoreLeft;
                else
                    ++_scoreRight;

                if (ScoreChanged != null)
                    ScoreChanged(_scoreLeft, _scoreRight);
            };
        }
    }
}