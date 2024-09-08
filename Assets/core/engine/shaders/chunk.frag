#version 330 core
in vec2 texCoord;
in float frag_lighting;
in vec4 Color;
out vec4 outputColor;

uniform sampler2D TextureAtlas;

void main()
{
    vec4 BlockTexture = texture(TextureAtlas,texCoord);
    //vec4 BreakingTexture = texture(TextureAtlas,brecoord);
    //vec4 TextColor = mix(BlockTexture ,BreakingTexture,BreakingTexture.a);
    vec4 TextColor = BlockTexture;
    vec3 frag_lightingvec = vec3(frag_lighting,frag_lighting,frag_lighting);
    vec3 LightColor = mix(vec3(0,0,0),TextColor.rgb ,frag_lighting);
    outputColor = vec4(LightColor, TextColor.a) * Color;
    if (TextColor.a == 0.0) discard;
}
