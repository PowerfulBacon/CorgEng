#version 330 core

in vec2 fragVertexUV;
in vec4 fragTextureData;

out vec4 result;

// The translation matrix (Model, View)
//uniform mat4 objectMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
 
void main()
{
    result = vec4(1, 0, 0, 1);
}