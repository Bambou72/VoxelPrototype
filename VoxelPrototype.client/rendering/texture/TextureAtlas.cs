﻿using OpenTK.Graphics.OpenGL4;
using VoxelPrototype.client.utils.StbImageSharp;
namespace VoxelPrototype.client.rendering.texture
{
    internal class TextureAtlas : ITexture
    {
        int Handle;

        Dictionary<string, float[]> Coordinates = new();

        internal void SetTexture(ImageResult Image)
        {
            Handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Image.Width, Image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Image.Data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.ActiveTexture(0);
        }
        public void AddCoordinates(string resourceLocation, float[] coordinates)
        {
            Coordinates[resourceLocation] = coordinates;
        }
        public float[] GetCoordinates(string resourceLocation)
        {
            if (Coordinates.TryGetValue(resourceLocation, out var coordinates))
            {
                return coordinates;
            }
            throw new Exception("Can't find requested texture coordinates");
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

        public int GetHandle()
        {
            return Handle;
        }
    }
}
