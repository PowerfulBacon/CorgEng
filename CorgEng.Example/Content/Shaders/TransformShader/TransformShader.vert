#version 330 core
layout (location = 0) in vec3 inVertexPos;

out vec2 fragUV;

void main()
{
	gl_Position = vec4(inVertexPos, 1.0);
	fragUV = inVertexPos.xy * 0.5 + 0.5;
}
