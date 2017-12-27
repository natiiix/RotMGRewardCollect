using System.Drawing; // Bitmap, Graphics
using System.Runtime.InteropServices; // DllImport
using System.Windows.Forms; // Screen
using System.Windows.Input; // Key

namespace WinApi
{
    public static class Keyboard
    {
        #region PInvoke

        // Used for simulation of keyboard events
        [DllImport("user32.dll")]
#pragma warning disable IDE1006 // Naming Styles
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

#pragma warning restore IDE1006 // Naming Styles

        #endregion PInvoke

        #region Constants

        //const uint KEYEVENTF_EXTENDEDKEY = 0x0001; // Used for sending numeric keys (not really necessary for anything)
        private const uint KEYEVENTF_KEYUP = 0x0002;

        #endregion Constants

        #region Press

        public static void Press(Key key)
        { Press((byte)KeyInterop.VirtualKeyFromKey(key)); }

        public static void Press(Keys key)
        { Press((byte)key); }

        public static void Press(byte key)
        {
            Down(key);
            Up(key);
        }

        #endregion Press

        #region Down

        public static void Down(Key key)
        { Down((byte)KeyInterop.VirtualKeyFromKey(key)); }

        public static void Down(Keys key)
        { Down((byte)key); }

        public static void Down(byte key)
        { keybd_event(key, 0, 0, 0); }

        #endregion Down

        #region Up

        public static void Up(Key key)
        { Up((byte)KeyInterop.VirtualKeyFromKey(key)); }

        public static void Up(Keys key)
        { Up((byte)key); }

        public static void Up(byte key)
        { keybd_event(key, 0, KEYEVENTF_KEYUP, 0); }

        #endregion Up
    }

    public static class Mouse
    {
        #region class NatiMouseButton

        // Used to distinguish between various mouse buttons
        private class NatiMouseButton
        {
            public uint Down = 0;
            public uint Up = 0;

            public NatiMouseButton(uint _down, uint _up)
            {
                Down = _down;
                Up = _up;
            }
        }

        #endregion class NatiMouseButton

        #region PInvoke

        // Used for moving the cursor
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        // Used for pressing and releasing mouse buttons
        [DllImport("user32.dll")]
#pragma warning disable IDE1006 // Naming Styles
        private static extern void mouse_event(uint dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

#pragma warning restore IDE1006 // Naming Styles

        #endregion PInvoke

        #region constants

        //private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;

        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;

        //private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;

        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        //private const uint MOUSEEVENTF_XDOWN = 0x0080;
        //private const uint MOUSEEVENTF_XUP = 0x0100;
        //private const uint MOUSEEVENTF_WHEEL = 0x0800;
        //private const uint MOUSEEVENTF_HWHEEL = 0x01000;

        private static NatiMouseButton BUTTON_LEFT = new NatiMouseButton(MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_LEFTUP);
        private static NatiMouseButton BUTTON_RIGHT = new NatiMouseButton(MOUSEEVENTF_RIGHTDOWN, MOUSEEVENTF_RIGHTUP);
        private static NatiMouseButton BUTTON_MIDDLE = new NatiMouseButton(MOUSEEVENTF_MIDDLEDOWN, MOUSEEVENTF_MIDDLEUP);

        private const int ACTION_CLICK = 0;
        private const int ACTION_DOWN = 1;
        private const int ACTION_UP = 2;

        #endregion constants

        #region solve

        private static void Solve(NatiMouseButton _button, int _action, int x = -1, int y = -1)
        {
            // If the mouse action is performed without moving the cursor, x and y are -1
            if (x >= 0 && y >= 0)
                Move(x, y);
            else
                x = y = 0;

            if (_action != ACTION_UP)
                mouse_event(_button.Down, x, y, 0, 0);

            if (_action != ACTION_DOWN)
                mouse_event(_button.Up, x, y, 0, 0);
        }

        #endregion solve

        #region Move

        // Moves the cursor to desired location
        public static void Move(Point p)
        { Move(p.X, p.Y); }

        public static void Move(int x, int y)
        { SetCursorPos(x, y); }

        #endregion Move

        #region Left Button

        // LeftClick
        public static void LeftClick(Point p)
        { LeftClick(p.X, p.Y); }

        public static void LeftClick(int x = -1, int y = -1)
        { Solve(BUTTON_LEFT, ACTION_CLICK, x, y); }

        // LeftDown
        public static void LeftDown(Point p)
        { LeftDown(p.X, p.Y); }

        public static void LeftDown(int x = -1, int y = -1)
        { Solve(BUTTON_LEFT, ACTION_DOWN, x, y); }

        // LeftUp
        public static void LeftUp(Point p)
        { LeftUp(p.X, p.Y); }

        public static void LeftUp(int x = -1, int y = -1)
        { Solve(BUTTON_LEFT, ACTION_DOWN, x, y); }

        #endregion Left Button

        #region Right Button

        // RightClick
        public static void RightClick(Point p)
        { RightClick(p.X, p.Y); }

        public static void RightClick(int x = -1, int y = -1)
        { Solve(BUTTON_RIGHT, ACTION_CLICK, x, y); }

        // RightDown
        public static void RightDown(Point p)
        { RightDown(p.X, p.Y); }

        public static void RightDown(int x = -1, int y = -1)
        { Solve(BUTTON_RIGHT, ACTION_DOWN, x, y); }

        // RightUp
        public static void RightUp(Point p)
        { RightUp(p.X, p.Y); }

        public static void RightUp(int x = -1, int y = -1)
        { Solve(BUTTON_RIGHT, ACTION_DOWN, x, y); }

        #endregion Right Button

        #region Middle Button

        // MiddleClick
        public static void MiddleClick(Point p)
        { MiddleClick(p.X, p.Y); }

        public static void MiddleClick(int x = -1, int y = -1)
        { Solve(BUTTON_MIDDLE, ACTION_CLICK, x, y); }

        // MiddleDown
        public static void MiddleDown(Point p)
        { MiddleDown(p.X, p.Y); }

        public static void MiddleDown(int x = -1, int y = -1)
        { Solve(BUTTON_MIDDLE, ACTION_DOWN, x, y); }

        // MiddleUp
        public static void MiddleUp(Point p)
        { MiddleUp(p.X, p.Y); }

        public static void MiddleUp(int x = -1, int y = -1)
        { Solve(BUTTON_MIDDLE, ACTION_DOWN, x, y); }

        #endregion Middle Button

        #region GetPosition

        public static Point GetPosition()
        {
            return Cursor.Position;
        }

        #endregion GetPosition
    }
}