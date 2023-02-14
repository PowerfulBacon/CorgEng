#version 330 core

in vec2 fragUV;

out vec4 result;

uniform sampler2D renderTexture;

void main()
{
	result = texture(renderTexture, fragUV);
	result.r = result.r > 1 ? 0 : result.r;
	result.g = result.g > 1 ? 0 : result.g;
	result.b = result.b > 1 ? 0 : result.b;
}
