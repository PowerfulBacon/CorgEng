#version 330 core

in vec2 fragUV;

out vec4 result;

uniform sampler2D renderTexture;
uniform sampler2D depthTexture;

uniform float depthIncrement = 0;

void main()
{
	result = texture(renderTexture, fragUV);
	float depthValue = texture(depthTexture, fragUV).r + depthIncrement;
	gl_FragDepth = depthValue;
	//result = texture(depthTexture, fragUV);
	//result.a *= depthValue;
}
