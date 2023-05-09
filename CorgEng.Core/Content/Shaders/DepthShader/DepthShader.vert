#version 330 core
layout (location = 0) in vec3 inVertexPos;

void main()
{
	gl_Position = vec4(inVertexPos, 1.0);
}
