using OpenTK.Graphics.OpenGL4;
using VoxelPrototype.client.utils.StbImageSharp;
namespace VoxelPrototype.client.rendering.texture
{
    public class CubeMapTexture
    {
        public readonly int Handle;
        public static CubeMapTexture LoadCubeMap(List<string> faces)
        {
            int handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, handle);
            StbImage.stbi_set_flip_vertically_on_load(1);
            TextureTarget[] targets =
            {
               TextureTarget.TextureCubeMapPositiveX, TextureTarget.TextureCubeMapNegativeX,
               TextureTarget.TextureCubeMapPositiveY, TextureTarget.TextureCubeMapNegativeY,
               TextureTarget.TextureCubeMapPositiveZ, TextureTarget.TextureCubeMapNegativeZ
            };
            for (int i = 0; i < 6; i++)
            {
                using Stream stream = File.OpenRead(faces[i]);
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(targets[i], 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            return new CubeMapTexture(handle);
        }
        public CubeMapTexture(int glHandle)
        {
            Handle = glHandle;
            //type
        }
        public void Use(TextureUnit Textu)
        {
            GL.ActiveTexture(Textu);
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }
    }
}