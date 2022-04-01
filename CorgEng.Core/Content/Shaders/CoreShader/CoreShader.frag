#version 330 core

in vec2 fragUV;

out vec4 result;

uniform sampler2D renderTexture;

void main()
{
	result = texture(renderTexture, fragUV);
	//result = vec4(1, 0, 0, 1);
}
