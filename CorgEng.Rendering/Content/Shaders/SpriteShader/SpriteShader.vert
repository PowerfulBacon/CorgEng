#version 330 core
//The vertex data relative to the model.
layout (location = 0) in vec3 inVertexPos;
layout (location = 1) in vec2 inVertexUV;
layout (location = 2) in vec3 inInstancePos;
layout (location = 4) in vec4 inTextureData;

//UV data
out vec2 fragVertexUV;
out vec4 fragTextureData;

// The translation matrix (Model, View)
//uniform mat4 objectMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
 
void main()
{
    //mat4 MVP = projectionMatrix * viewMatrix;
    //gl_Position = MVP * vec4(inInstancePos + inVertexPos, 1.0);
    gl_Position = vec4(inVertexPos, 1.0);
    //Output the vertex UV to the fragment shader
    fragVertexUV = inVertexUV;
    fragTextureData = inTextureData;
}