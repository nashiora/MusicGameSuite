#version 330
#extension GL_ARB_separate_shader_objects : enable

layout (location = 1) in vec2 frag_TexCoord;
layout (location = 0) out vec4 target;

uniform sampler2D MainTexture;
uniform vec4 Color;

uniform vec3 LaserColor;
uniform vec3 HiliteColor;

void main()
{	
	vec4 s = texture(MainTexture, frag_TexCoord);
	target = vec4(s.g * LaserColor + 0.5 * s.r * HiliteColor, s.a);
}