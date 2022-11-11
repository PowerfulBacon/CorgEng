#version 330 core

in vec2 uv;

out vec4 result;

uniform int pixelWidth;
uniform int pixelHeight;

uniform sampler2D sampler;
uniform vec4 iconOffset;

void main()
{
    vec2 transformedUV = uv;
    //transformedUV *= 0.125;
    transformedUV *= vec2(iconOffset[2], iconOffset[3]);
    transformedUV += vec2(iconOffset[0], 1.0 - iconOffset[1] - iconOffset[3]);
    result = texture(sampler, transformedUV);
    if (result.a == 0)
    {
        discard;
    }
}
