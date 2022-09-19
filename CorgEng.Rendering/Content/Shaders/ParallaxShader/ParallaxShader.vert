#version 330 core
//The vertex data relative to the model.
layout (location = 0) in vec3 inVertexPos;
layout (location = 1) in vec2 inVertexUV;
layout (location = 2) in vec3 inMatrixFirst;
layout (location = 3) in vec3 inMatrixSecond;
layout (location = 4) in vec4 inTextureData;
layout (location = 5) in float inLayer;

//UV data
out vec2 fragVertexUV;
out vec4 fragTextureData;
 
void main()
{
	gl_Position = vec4(2.0 * inVertexPos, 1.0);
    //Output the vertex UV to the fragment shader
    fragVertexUV = inVertexUV;
    fragTextureData = inTextureData;
}