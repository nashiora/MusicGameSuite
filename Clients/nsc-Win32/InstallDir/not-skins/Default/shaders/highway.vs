#version 330
#extension GL_ARB_separate_shader_objects : enable

layout (location = 0) in vec3 in_Position;
layout (location = 1) in vec2 in_TexCoord;

out gl_PerVertex
{
	vec4 gl_Position;
};

layout (location = 1) out vec2 frag_TexCoord;

uniform mat4 Projection;
uniform mat4 Camera;
uniform mat4 World;

void main()
{
	frag_TexCoord = in_TexCoord;
	gl_Position = Projection * Camera * World * vec4(in_Position.xyz, 1);
}