#version 330 core
//The vertex data relative to the model.
layout (location = 0) in vec3 pos;
layout (location = 1) in vec2 vertexUv;

//UV data
out vec2 UV;

// The translation matrix (Model, View)
uniform mat4 objectMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
 
void main()
{
    //Calculate the MVP
    mat4 MVP = projectionMatrix * viewMatrix * objectMatrix;
    //Set the gl position
    gl_Position = MVP * vec4(pos, 1.0);
    //Output the vertex UV to the fragment shader
    UV = vertexUv;
}