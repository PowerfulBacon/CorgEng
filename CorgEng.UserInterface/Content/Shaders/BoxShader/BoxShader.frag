#version 330 core

in vec2 uv;

out vec4 result;

uniform int pixelWidth;
uniform int pixelHeight;

uniform float uniformLocationBorderWidth;
uniform vec3 uniformLocationBorderColour;
uniform vec3 uniformLocationFillColour;

void main()
{
    //Calculate border size
    float borderWidth = uniformLocationBorderWidth / pixelWidth;
    float borderHeight = uniformLocationBorderWidth / pixelHeight;
    result = vec4(borderWidth, borderHeight, 0, 1);
    int isBorder = (uv.x < borderWidth || uv.x > 1.0 - borderWidth || uv.y < borderHeight || uv.y > 1.0 - borderHeight) ? 1 : 0;
    //result = vec4(uv, 0, 1);
    //Choose the colour
    result = vec4((1 - isBorder) * uniformLocationFillColour.rgb + isBorder * uniformLocationBorderColour.rgb, 1);
}
