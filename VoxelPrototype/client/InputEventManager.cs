using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;
namespace VoxelPrototype.client
{
    internal class InputEventManager
    {
        public  bool Grab = false;
        public  bool NoInput = false;
        public  double DT;
        List<Func<MouseDownEvent,bool>> OnMouseDownCallbacks = new List<Func<MouseDownEvent, bool>>();
        List<Func<MouseUpEvent,bool>> OnMouseUpCallbacks = new List<Func<MouseUpEvent, bool>>();
        List<Func<MouseClickedEvent, bool>> OnMouseClickedCallbacks = new List<Func<MouseClickedEvent, bool>>();
        List<Func<MouseMovedEvent, bool>> OnMouseMoveCallbacks = new List<Func<MouseMovedEvent, bool>>();
        List<Func<MouseWheelEvent, bool>> OnMouseWheelCallbacks = new List<Func<MouseWheelEvent, bool>>();
        List<Func<KeyDownEvent,bool>> OnKeyDownCallbacks = new List<Func<KeyDownEvent, bool>>();
        List<Func<KeyUpEvent,bool>> OnKeyUpCallbacks = new List<Func<KeyUpEvent, bool>>();
        List<Func<KeyPressedEvent,bool>> OnKeyPressedCallbacks = new List<Func<KeyPressedEvent, bool>>();
        private Dictionary<Keys, bool> prevKeys;
        private Dictionary<Keys, bool> currentKeys;
        private bool[] prevButtons;
        private bool[] currentButtons;
        public InputEventManager()
        {
            prevKeys = new Dictionary<Keys, bool>();
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                prevKeys[key] = false;
            }
            currentKeys = new Dictionary<Keys, bool>();
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                currentKeys[key] = false;
            }
            int numButtons = Enum.GetNames(typeof(MouseButton)).Length;
            prevButtons = new bool[numButtons];
            currentButtons = new bool[numButtons];
        }

        internal void OnMouseDown(MouseButton Button,Vector2i MousePos)
        {
            currentButtons[(int)Button] = true;
            if (prevButtons[(int)Button] == false)
            {
                var MouseClickedEvent = new MouseClickedEvent
                {
                    Button = Button,
                    MousePosition = MousePos
                };
                foreach (var action in OnMouseClickedCallbacks)
                {
                    if (action(MouseClickedEvent))
                    {
                        return;
                    }
                }
            }
            
            
        }
        internal void OnMouseUp(MouseButton Button, Vector2i MousePos)
        {
            currentButtons[(int)Button] = false;
            var MouseUpEvent = new MouseUpEvent
            {
                MousePosition = MousePos,
                Button = Button,
            };
            foreach (var action in OnMouseUpCallbacks)
            {
                if (action(MouseUpEvent))
                {
                    return;
                }
            }
        }

        internal void OnMouseMove(Vector2i MousePos, Vector2i MouseDelta)
        {
            var MouseMoveEvent = new MouseMovedEvent
            {
                MousePosition = MousePos,
                Delta = MouseDelta,
            };
            foreach (var action in OnMouseMoveCallbacks)
            {
                if (action(MouseMoveEvent))
                {
                    return;
                }
            }


        }
        internal void OnMouseWheel( Vector2i MouseWheelDelta)
        {
            var MouseWheelEvent = new MouseWheelEvent
            {
                Delta = MouseWheelDelta,
            };
            foreach (var action in OnMouseWheelCallbacks)
            {
                if (action(MouseWheelEvent))
                {
                    return;
                }
            }
        }
        internal void OnKeyDown(Keys Key,KeyModifiers Modifiers)
        {
            currentKeys[Key] = true;
            if (prevKeys[Key] == false)
            {
                var KeyPressedEvent = new KeyPressedEvent
                {
                    Key = Key,
                    Modifiers = Modifiers
                };
                foreach (var action in OnKeyPressedCallbacks)
                {
                    if (action(KeyPressedEvent))
                    {
                        return;
                    }
                }
            }else
            {
                var KeyDownEvent = new KeyDownEvent
                {
                    Key = Key,
                    Modifiers = Modifiers
                };
                foreach (var action in OnKeyDownCallbacks)
                {
                    if (action(KeyDownEvent))
                    {
                        return;
                    }
                }
            }
        }
        internal bool IsKeyPressed(Keys Key)
        {
            if (currentKeys[Key] == true && prevKeys[Key] == false)
            {
                return true;
            }
            return false;
        }
        internal bool IsKeyDown(Keys Key)
        {
            return currentKeys[Key] == true;
        }
        internal bool IsMouseButtonPressed(MouseButton Button)
        {
            if (currentButtons[(int)Button] == true && prevButtons[(int)Button] == false)
            {
                return true;
            }
            return false;
        }
        internal bool IsMouseButtonDown(MouseButton Button)
        {
            return currentButtons[(int)Button] == true;
        }
        internal void OnKeyUp(Keys Key, KeyModifiers Modifiers)
        {
            currentKeys[Key] = false;

            var KeyUpEvent = new KeyUpEvent
            {
                Key = Key,
                Modifiers = Modifiers
            };
            foreach (var action in OnKeyUpCallbacks)
            {
                if (action(KeyUpEvent))
                {
                    return;
                }
            }
        }
        public void CopyCurrentToPrevious()
        {
            prevKeys.Clear();
            foreach (var kvp in currentKeys)
            {
                prevKeys[kvp.Key] = kvp.Value;
            }
        }
        public void Update()
        {
            foreach(MouseButton but in Enum.GetValues(typeof(MouseButton)))
            {
                if (currentButtons[  ( int)but])
                {
                    var MouseDownEvent = new MouseDownEvent
                    {
                        Button = but,
                        MousePosition = (Vector2i)Client.TheClient.MousePosition,
                    };
                    foreach (var action in OnMouseDownCallbacks)
                    {
                        if (action(MouseDownEvent))
                        {
                            return;
                        }
                    }
                }
            }
            CopyCurrentToPrevious();
            Array.Copy(currentButtons, prevButtons, currentButtons.Length);
        }
        public void RegisterOnMouseMoveCallback(Func<MouseMovedEvent,bool> Callback)
        {
            OnMouseMoveCallbacks.Add(Callback);
        }
        public void RegisterOnMouseWheelCallback(Func<MouseWheelEvent, bool> Callback)
        {
            OnMouseWheelCallbacks.Add(Callback);
        }
        public void RegisterOnMouseDownCallback(Func<MouseDownEvent, bool> Callback)
        {
            OnMouseDownCallbacks.Add(Callback);
        }
        public void RegisterOnMouseUpCallback(Func<MouseUpEvent, bool> Callback)
        {
            OnMouseUpCallbacks.Add(Callback);
        }
        public void RegisterOnMouseClickedCallback(Func<MouseClickedEvent, bool> Callback)
        {
            OnMouseClickedCallbacks.Add(Callback);
        }
        public void RegisterOnKeyDownCallback(Func<KeyDownEvent, bool> Callback)
        {
            OnKeyDownCallbacks.Add(Callback);
        }
        public void RegisterOnKeyPressedCallback(Func<KeyPressedEvent, bool> Callback)
        {
            OnKeyPressedCallbacks.Add(Callback);
        }
        internal  bool GetGrab()
        {
            return Grab;
        }
        internal  bool GetNoInput()
        {
            return false;
        }

    }
}
