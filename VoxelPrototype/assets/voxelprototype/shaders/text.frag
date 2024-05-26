#version 330

uniform sampler2D FontAtlas;

in vec2 TextureCoords;
in vec4 Color;

out vec4 OutColor;
void main(void)
{
    float opacity = texture(FontAtlas,TextureCoords).r;
    OutColor = vec4(Color.xyz,opacity);
}