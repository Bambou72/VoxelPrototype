#version 460 core

layout(location = 0) in vec3 Vertex;
layout(location = 1) in vec2 Texture;
layout(location = 2) in int AO;
layout(location = 3) in uint Colors;

out vec2 texCoord;
out float frag_lighting;
out vec4 Color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
const float AOcurve[] = float[](0.3, 0.50, 0.75, 1.0);
vec4 intToColor(uint color) {
    float r = float((color >> 24) & 0xFFu) / 255.0;
    float g = float((color >> 16) & 0xFFu) / 255.0;
    float b = float((color >> 8) & 0xFFu) / 255.0;
    float a = float(color & 0xFFu) / 255.0;
    return vec4(r, g, b, a);
}
void main(void)
{
    texCoord = Texture;
    frag_lighting = AOcurve[AO];
    Color = intToColor(Colors);
    gl_Position = vec4(Vertex, 1.0) * model * view * projection;
}