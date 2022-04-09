#version 330 core

in vec2 fragVertexUV;
in vec4 fragTextureData;

out vec4 result;

// The translation matrix (Model, View)
//uniform mat4 objectMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
 
uniform sampler2D renderTexture;

void main()
{
    vec2 transformedUV = fragVertexUV;
    //transformedUV *= 0.125;
    transformedUV *= vec2(fragTextureData[2], fragTextureData[3]);
    transformedUV += vec2(fragTextureData[0], 1.0 - fragTextureData[1] - fragTextureData[3]);
    result = texture(renderTexture, transformedUV);
    //result = vec4(1, 0, 0, 1);
}