#version 330 core

in vec2 fragUV;

out vec4 result;

uniform sampler2D renderTexture;
uniform sampler2D depthTexture;

void main()
{
	result.rgb = vec3(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z);
}
