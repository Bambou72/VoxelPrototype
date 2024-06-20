using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Client
{
    internal class MouseListener
    {
        private double ScrollX,ScrollY =0;
        private double X,Y,LastX,LastY = 0;
        private bool[] MouseButtonDown = new bool[3];
        private bool[] OldMouseButtonDown = new bool[3];
        private bool _IsDragging;
        internal void MousePosition(double NewX, double BewY)
        {
            LastX = X;
            LastY  = Y;
            X = NewX;
            Y = BewY;
            _IsDragging = MouseButtonDown[0] || MouseButtonDown[1] || MouseButtonDown[2];
        }
        public void MouseButtonCallback(MouseButton button, InputAction act, KeyModifiers mod)
        {
            if((int)button < MouseButtonDown.Length)
            {
                if (act == InputAction.Press)
                {
                    MouseButtonDown[(int)button] = true;
                }
                else if (act == InputAction.Release)
                {
                    MouseButtonDown[(int)button] = false;
                    _IsDragging = false;
                }

            }
        }
        public void MouseScrollCallback(double x, double y)
        {
            ScrollX  = x; ScrollY = y;
        }
        public void EndFrame()
        {
            ScrollX = 0; ScrollY = 0;
            LastX = X; LastY = Y;
            Array.Copy(MouseButtonDown, OldMouseButtonDown, MouseButtonDown.Length);

        }
        public Vector2d GetMousePosition()
        {
            return new Vector2d(X,Y);
        }
        public Vector2d GetMouseDelta()
        {
            return new Vector2d(X -LastX , Y - LastY);
        }
        public Vector2d GetScroll()
        {
            return new Vector2d(ScrollX, ScrollY);
        }
        public bool IsDragging()
        {
            return _IsDragging;
        }
        public bool IsMouseButtonDown(MouseButton button)
        {
            if((int)button < MouseButtonDown.Length)
            {
                return MouseButtonDown[(int)button];
            }
            return false;
        }
        public bool IsMouseButtonPressed(MouseButton button)
        {
            if ((int)button < MouseButtonDown.Length)
            {
                return MouseButtonDown[(int)button] && !OldMouseButtonDown[(int)button];
            }
            return false;
        }
    }
}
