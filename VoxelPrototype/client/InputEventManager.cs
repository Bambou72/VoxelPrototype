using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.GUI.Elements;
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
        private bool[] prevKeys;
        private bool[] currentKeys;
        private bool[] prevButtons;
        private bool[] currentButtons;
        public InputEventManager()
        {
            int numKeys = Enum.GetValues(typeof(Keys)).Length;
            prevKeys = new bool[numKeys];
            currentKeys = new bool[numKeys];
            int numButtons = Enum.GetValues(typeof(MouseButton)).Length;
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
            currentKeys[(int)Key] = true;
            if (prevKeys[(int)Key] == false)
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
        internal void OnKeyUp(Keys Key, KeyModifiers Modifiers)
        {
            currentKeys[(int)Key] = false;

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
        public void Update()
        {
            foreach(MouseButton but in Enum.GetValues(typeof(MouseButton)))
            {
                if (currentButtons[(int)but])
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
            Array.Copy(currentKeys, prevKeys, currentKeys.Length);
            currentKeys = new bool[Enum.GetValues(typeof(Keys)).Length];
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
        //
    }
}
