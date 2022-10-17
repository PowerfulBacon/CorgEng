#version 330 core
//The vertex data relative to the model.
layout (location = 0) in vec3 inVertexPos;
layout (location = 1) in vec2 inVertexUV;
layout (location = 2) in vec3 inMatrixFirst;
layout (location = 3) in vec3 inMatrixSecond;
layout (location = 4) in vec4 inTextureData;
layout (location = 5) in float inLayer;
layout (location = 6) in vec4 inColour;

//UV data
out vec2 fragVertexUV;
out vec4 fragTextureData;
out vec4 fragColour;

// The translation matrix (Model, View)
//uniform mat4 objectMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
 
void main()
{
	//Create the matrix
	mat4 transformationMatrix = mat4(
		inMatrixFirst.x, inMatrixSecond.x, 0, 0,
		inMatrixFirst.y, inMatrixSecond.y, 0, 0,
		0, 0, 1, 0,
		inMatrixFirst.z, inMatrixSecond.z, 1-inLayer, 1
	);
    gl_Position = projectionMatrix * viewMatrix * transformationMatrix * vec4(inVertexPos, 1.0);
    //Output the vertex UV to the fragment shader
    fragVertexUV = inVertexUV;
    fragTextureData = inTextureData;
	fragColour = inColour;
}