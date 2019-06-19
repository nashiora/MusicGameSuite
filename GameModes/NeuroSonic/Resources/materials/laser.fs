#version 330
#extension GL_ARB_separate_shader_objects : enable

layout (location = 1) in vec2 frag_TexCoord;
layout (location = 0) out vec4 target;

uniform sampler2D MainTexture;

uniform vec3 LaserColor;
uniform vec3 HiliteColor;

uniform float Glow;
uniform int GlowState;

void main()
{	
	vec3 s = texture(MainTexture, frag_TexCoord).rgb;
	vec3 color = vec3(s.g * LaserColor + 0.5 * s.r * HiliteColor);

	target = vec4(color, 1);
}