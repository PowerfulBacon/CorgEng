#version 330 core

in vec2 fragUV;

out vec4 result;

uniform sampler2D renderTexture;
uniform sampler2D depthTexture;

void main()
{
	result = texture(renderTexture, fragUV);
	//float depthValue = texture(depthTexture, fragUV).r;
	//result.a *= depthValue;
}
