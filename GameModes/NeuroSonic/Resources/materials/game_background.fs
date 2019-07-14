#version 330
#extension GL_ARB_separate_shader_objects : enable

layout (location = 1) in vec2 frag_TexCoord;
layout (location = 0) out vec4 target;

uniform sampler2D MainTexture;
uniform vec4 Color;

void main()
{	
	target = vec4(0.25, 0.7, 1, 1);
}