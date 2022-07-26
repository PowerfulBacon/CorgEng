#version 330 core
//The vertex data relative to the model.
layout (location = 0) in vec3 inVertexPos;
layout (location = 1) in vec2 inVertexUV;
 
out vec2 uv;

void main()
{
    uv = inVertexUV;
    gl_Position = vec4(inVertexPos * 2, 1.0);
}
