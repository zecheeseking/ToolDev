using SharpDX;
using SharpDX.DirectInput;

namespace ToolDev_IvyGenerator.Utilities
{
    class InputManager
    {
        private static InputManager _instance;

        public static InputManager Instance => _instance ?? (_instance = new InputManager());

        private readonly Mouse _mouse;
        private MouseState _mouseState;
        private readonly Keyboard _kb;
        private KeyboardState _kbState;

        private InputManager()
        {
            var di = new DirectInput();
            _mouse = new Mouse(di);
            _mouse.Acquire();
            _mouseState = _mouse.GetCurrentState();
            _kb = new Keyboard(di);
            _kb.Acquire();
            _kbState = _kb.GetCurrentState();
        }

        public MouseState GetCurrentMouseState()
        {
            return _mouseState;
        }

        public KeyboardState GetCurrenKeyboardState()
        {
            return _kbState;
        }

        public void Update()
        {
            _mouseState = _mouse.GetCurrentState();
            _kbState = _kb.GetCurrentState();
        }

        public bool GetMouseButton(int index)
        {
            return _mouse.GetCurrentState().Buttons[index];
        }

        public Vector2 GetMouseDelta()
        {
            var mouseDelta = new Vector2(_mouseState.X, _mouseState.Y);

            return mouseDelta;
        }
    }
}
