#version 330 core

in vec2 fragVertexUV;
in vec4 fragTextureData;

// The translation matrix (Model, View)
//uniform mat4 objectMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
 
void main()
{
    gl_FragColor = vec4(1, 0, 0, 1);
}