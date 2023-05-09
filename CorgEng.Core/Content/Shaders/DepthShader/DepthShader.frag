#version 330 core

out vec4 result;

void main()
{
	result.rgb = vec3(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z);
}
