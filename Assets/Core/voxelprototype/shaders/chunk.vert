#version 460 core

layout(location = 0) in vec3 Vertex;
layout(location = 1) in vec2 Texture;
layout(location = 2) in int AO;

out vec2 texCoord;
out float frag_lighting;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
const float AOcurve[] = float[](0.25, 0.5, 0.75, 1.0);
void main(void)
{
    texCoord = Texture;
    frag_lighting = AOcurve[AO];
    gl_Position = vec4(Vertex, 1.0) * model * view * projection;
}