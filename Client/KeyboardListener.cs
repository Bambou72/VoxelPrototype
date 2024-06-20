
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Client
{
    internal class KeyboardListener
    {
        private bool[] KeysDown = new bool[350];
        private bool[] OldKeysDown = new bool[350];
        internal void KeyCallback(Keys key, int scanCode, InputAction act, KeyModifiers mod)
        {
            if(act == InputAction.Press)
            {
                KeysDown[(int)key] = true;
            }else if(act == InputAction.Release)
            {
                KeysDown[(int)key] = false;
            }
        }
        internal bool IsKeyDown(Keys key)
        {
            return KeysDown[(int)key];
        }
        internal bool IsKeyPressed(Keys key)
        {
            return KeysDown[(int)key] && !OldKeysDown[(int)key];
        }
        internal void EndFrame()
        {
            Array.Copy(KeysDown, OldKeysDown, KeysDown.Length);
        }
    }
}
