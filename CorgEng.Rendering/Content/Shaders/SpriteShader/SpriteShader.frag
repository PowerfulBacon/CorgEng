#version 330 core

in vec2 fragVertexUV;
in vec4 fragTextureData;
in vec4 fragColour;

out vec4 result;

in float debug;

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
    result = texture(renderTexture, transformedUV) * fragColour;
    //result.rgb = vec3(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z);
    //result = vec4(1, 0, 0, 1);
    //result = vec4(vec3(gl_FragCoord.z), 1.0);
    if (result.a == 0)
    {
        discard;
    }
}