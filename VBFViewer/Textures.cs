/**
 * Texture class
 * Authors Opentk
 **/
using OpenTK.Graphics.OpenGL4;
namespace VBFViewer
{
    // A helper class, much like Shader, meant to simplify loading textures.
    public class Texture 
    {
        internal readonly int Handle;
        internal Texture(int glHandle)
        {
            Handle = glHandle;
        }
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
        public void Clean()
        {
            GL.DeleteTexture(Handle);
        }
    }
}