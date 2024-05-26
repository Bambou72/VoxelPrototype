#version 460  core
layout (location = 0) in vec3 aPos;

out vec3 TexCoords;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main()
{
    
    vec4 MVP_Pos = vec4(aPos, 0) * view * projection;
    gl_Position = MVP_Pos.xyww;	
    TexCoords = vec3(aPos.x,-aPos.y,aPos.z);
}  