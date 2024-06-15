#version 330

layout(location = 0) in vec2 Pos;
layout(location = 1) in vec4 Colors;
layout(location = 2) in vec2 TexCoords;

uniform mat4 projection;

out vec2 TextureCoords;
out vec4 Color;

void main(void){
	gl_Position = vec4(Pos,0, 1.0) * projection;
	TextureCoords = TexCoords;
	Color = Colors;
}