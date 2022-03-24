#version 330 core

in vec2 UV;

out vec4 result;

uniform vec4 colour;

void main()
{
    result = colour;
} 