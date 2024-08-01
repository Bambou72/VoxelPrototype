#version 330
layout(location = 0) in vec2 Pos;
layout(location = 1) in vec2 TexCoords;
layout(location = 2) in uint Colors;
uniform mat4 projection;
out vec2 TextureCoords;
out vec4 Color;
vec4 intToColor(uint color) {
    float r = float((color >> 24) & 0xFFu) / 255.0;
    float g = float((color >> 16) & 0xFFu) / 255.0;
    float b = float((color >> 8) & 0xFFu) / 255.0;
    float a = float(color & 0xFFu) / 255.0;
    return vec4(r, g, b, a);
}
void main(){
	gl_Position = vec4(Pos,0, 1.0) * projection;
	TextureCoords = TexCoords;
	Color = intToColor(Colors);
}