using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.Rendering.Models
{
    public class Model : IModel
    {

        [UsingDependency]
        private static ILogger Logger;

        public int VertexCount { get; private set; }

        public uint VertexArrayObject { get; private set; }

        public uint VertexBuffer { get; private set; }

        public uint UvBuffer { get; private set; }

        public unsafe void GenerateBuffers(float[] vertices, float[] uvs)
        {
            //Length
            VertexCount = vertices.Length;

            //Generate the VAO
            VertexArrayObject = glGenVertexArray();
            glBindVertexArray(VertexArrayObject);

            //Generate the VBO
            VertexBuffer = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, VertexBuffer);

            //Fill the vertex buffer object with data about our vertices.
            fixed (float* v = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
            }

            //Generate the UV buffer
            UvBuffer = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, UvBuffer);

            //Generate the UV Buffer data
            fixed (float* u = &uvs[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * uvs.Length, u, GL_STATIC_DRAW);
            }

            Logger.WriteLine($"Generated buffer for model, VAO: {VertexArrayObject}, VBO: {VertexBuffer}, Length: {VertexCount}", LogType.DEBUG);
        }

    }
}
