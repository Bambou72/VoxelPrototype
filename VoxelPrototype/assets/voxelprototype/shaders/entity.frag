#version 440

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;


void main()
{
    vec4 Texture = texture(texture0,texCoord);
    outputColor = Texture;
    if (Texture.a == 0.0) { // discard if texel's alpha component is 0 (texel is transparent)
	discard;
    }
}