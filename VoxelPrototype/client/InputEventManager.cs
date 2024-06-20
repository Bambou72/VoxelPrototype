using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;
namespace VoxelPrototype.client
{
    
    internal class InputEventManager
    {
        public  bool Grab = false;
        public  bool NoInput = false;
        public InputEventManager()
        { 
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
