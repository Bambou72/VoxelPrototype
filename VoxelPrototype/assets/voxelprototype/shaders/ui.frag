#version 330
uniform sampler2D Texture;
in vec2 TextureCoords;
in vec4 Color;
out vec4 OutColor;
void main()
{
    OutColor = Color* texture(Texture, TextureCoords);
    if(OutColor.a == 0)
    {
        discard;
    }
}