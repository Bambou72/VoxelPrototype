/**
 * Texture class
 * Authors Opentk
 **/
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace VoxelPrototype.client.rendering.texture
{
    // A helper class, much like Shader, meant to simplify loading textures.
    public class Texture : ITexture
    {
        internal readonly int Handle;
        internal Texture(Vector2i Size, bool GenerateMipmap = true, TextureMinFilter MinFilter = TextureMinFilter.Nearest, TextureMagFilter MagFilter = TextureMagFilter.Nearest)
        {
            Handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Size.X, Size.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, nint.Zero);
            if (GenerateMipmap)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MinFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MagFilter);


            }
        }
        internal Texture(int glHandle)
        {
            Handle = glHandle;
        }
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
        public void SetData(byte[] Data, int Width, int Height, PixelInternalFormat PixelInternalFormat = PixelInternalFormat.Rgba, PixelFormat Format = PixelFormat.Rgba, bool GenerateMipmap = true, TextureMinFilter MinFilter = TextureMinFilter.Nearest, TextureMagFilter MagFilter = TextureMagFilter.Nearest)
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat, Width, Height, 0, Format, PixelType.UnsignedByte, Data);
            if (GenerateMipmap)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MinFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MagFilter);

            }

        }
        public void Clean()
        {
            GL.DeleteTexture(Handle);
        }

        public int GetHandle()
        {
            return Handle;
        }
    }
}