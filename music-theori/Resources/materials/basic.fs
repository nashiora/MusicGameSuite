#version 330
#extension GL_ARB_separate_shader_objects : enable

layout (location = 1) in vec2 frag_TexCoord;
layout (location = 0) out vec4 target;

uniform sampler2D MainTexture;
uniform vec4 Color;

void main()
{	
	vec4 mainColor = texture(MainTexture, frag_TexCoord);
	target = mainColor * Color;
}