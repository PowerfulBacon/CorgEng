#version 330 core

in vec2 fragVertexUV;
in vec4 fragTextureData;

out vec4 result;

in float debug;

// The translation matrix (Model, View)
//uniform mat4 objectMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
 
uniform sampler2D renderTexture;

const float parallaxZoomFactor = 0.01;
const float parallaxMoveFactor = 0.005;

void main()
{
    float viewX = sqrt(viewMatrix[0][0] * parallaxZoomFactor) + 1;
    float viewY = sqrt(viewMatrix[1][1] * parallaxZoomFactor) + 1;
    float translateX = viewMatrix[3][0] * parallaxMoveFactor;
    float translateY = viewMatrix[3][1] * parallaxMoveFactor;
    vec2 transformedUV = vec2(-translateX + (fragVertexUV.x - 0.5) / viewX, -translateY + (fragVertexUV.y - 0.5) / viewY);
    //transformedUV *= 0.125;
    transformedUV *= vec2(fragTextureData[2], fragTextureData[3]);
    transformedUV += vec2(fragTextureData[0], 1.0 - fragTextureData[1] - fragTextureData[3]);
    result = texture(renderTexture, transformedUV);
}