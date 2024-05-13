using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.CompilerServices;
using VoxelPrototype.API;
using VoxelPrototype.client.GUI;
namespace VoxelPrototype.client
{
    internal static class InputSystem
    {
        public static KeyboardState Keyboard;
        public static MouseState Mouse;
        public static bool Grab = false;
        public static bool NoInput = false;
        public static double DT;
        internal static bool GetGrab()
        {
            return Grab;
        }
        internal static bool GetNoInput()
        {
            return false;
        }
        //
        //Warning remove same class instance resetification
        //
        internal static void Update(KeyboardState K, MouseState M, double Dt)
        {
            DT = Dt;
            Keyboard = K;
            Mouse = M;
            if (Keyboard.IsKeyPressed(Keys.Escape) && !GUIVar.ConsoleMenu && Client.TheClient.World.Initialized)
            {
                if (GUIVar.IngameMenu)
                {
                    GUIVar.IngameMenu = false;
                    ClientAPI.SetCursorState(CursorState.Grabbed);
                    Grab = true;
                }
                else
                {
                    GUIVar.IngameMenu = true;
                    ClientAPI.SetCursorState(CursorState.Normal);
                    Grab = false;
                }
            }
            if (Keyboard.IsKeyPressed(Keys.F2))
            {
                if (Grab == false)
                {
                    ClientAPI.SetCursorState(CursorState.Grabbed);
                    Grab = true;
                }
                else
                {
                    ClientAPI.SetCursorState(CursorState.Normal);
                    Grab = false;
                }
            }
            if (Keyboard.IsKeyPressed(Keys.F1))
            {
                if (GUIVar.DebugMenu == false)
                {
                    GUIVar.DebugMenu = true;
                }
                else
                {
                    GUIVar.DebugMenu = false;
                }
            }
        }
        internal static bool KeyPressed(Keys key)
        {
            return Keyboard.IsKeyPressed(key);
        }
        internal static bool MousePressed(MouseButton but)
        {
            return Mouse.IsButtonPressed(but);
        }
        internal static bool MouseDown(MouseButton but)
        {
            return Mouse.IsButtonDown(but);
        }
        internal static bool KeyDown(Keys key)
        {
            return Keyboard.IsKeyDown(key);
        }
    }
}
