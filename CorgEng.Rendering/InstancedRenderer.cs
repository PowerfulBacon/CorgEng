using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Renderers;
using CorgEng.GenericInterfaces.Rendering.RenderObjects;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.Rendering
{

    internal abstract class InstancedRenderer<TSharedRenderAttributes, TBatch> : IRenderer
        where TSharedRenderAttributes : ISharedRenderAttributes
        where TBatch : IBatch<TBatch>, new()
    {

        // The amount of required attrib arrays (2: Vertices and UVs)
        protected const int USER_BUFFER_OFFSET = 2;

        // The shader set beloning to this renderer
        protected abstract IShaderSet ShaderSet { get; }

        //The renderer's cache
        protected Dictionary<ISharedRenderAttributes, TBatch> RenderCache { get; set; } = new Dictionary<ISharedRenderAttributes, TBatch>();

        //The uint for the render program
        protected uint programUint;

        //The location of buffers that we are using
        protected uint[] storedBufferLocations;

        public void Initialize()
        {
            CreateShaders();
            //Create a program for the renderer
            programUint = glCreateProgram();
            //Setup the program:
            //Start using the program
            glUseProgram(programUint);
            //Attach the shader set to the program to fetch uniform location variables
            ShaderSet.AttachShaders(programUint);
            //Link the program
            glLinkProgram(programUint);
            //Load the uniform variables in here, if required
            LoadUniformVariableLocations();
        }

        protected abstract void CreateShaders();

        public void Render(ICamera camera)
        {
            //Start using our program
            //Shaders were loaded during init
            glUseProgram(programUint);
            //Bind uniform variables
            BindUniformVariables(camera);
            //Perform actual rendering
            //Lock the render cache (Major rendering changes will wait for update completion)
            lock (RenderCache)
            {
                foreach (ISharedRenderAttributes sharedRenderAttribute in RenderCache.Keys)
                {
                    TBatch renderBatch = RenderCache[sharedRenderAttribute];
                    //Batch is empty, continue
                    if (renderBatch.Count == 0)
                        continue;

                    //====================
                    //Bind default attributes: Model and UV
                    //====================
                    BindAttribArray(0, sharedRenderAttribute.Model.VertexBuffer, 3);
                    BindAttribArray(1, sharedRenderAttribute.Model.UvBuffer, 2);
                    //Set the attrib divisors
                    glVertexAttribDivisor(0, 0);
                    glVertexAttribDivisor(1, 0);

                    //====================
                    // Bind custom attributes (Instance position etc.)
                    //====================
                    //Generate custom buffer locations if we need to
                    GenerateCustomBufferLocations(renderBatch);
                    for (uint i = USER_BUFFER_OFFSET; i < renderBatch.BatchVectorSizes.Length + 2; i++)
                    {
                        BindAttribArray(i, storedBufferLocations[i - 2], renderBatch.BatchVectorSizes[i - 2]);
                        glVertexAttribDivisor(i, 1);
                    }

                    //====================
                    //Deal with rendering
                    //====================
                    for (int i = renderBatch.IndividualBatchCounts - 1; i >= 0; i--)
                    {
                        int count = renderBatch.BatchSize;
                        //Calculate count
                        if (i == renderBatch.IndividualBatchCounts - 1)
                        {
                            count = renderBatch.Count % renderBatch.BatchSize;
                        }
                        //Bind and prepare a batch for rendering
                        BindBatchGroup(renderBatch, i, count);
                        //Do instanced rendering
                        glDrawArraysInstanced(GL_TRIANGLES, 0, sharedRenderAttribute.Model.VertexCount, count);
                    }
                }

                //====================
                //Unbind attrib arrays
                //====================
                for (uint i = 0; i < storedBufferLocations.Length + USER_BUFFER_OFFSET; i++)
                {
                    glDisableVertexAttribArray(i);
                }

            }
        }

        private int viewMatrixUniformLocation;
        private int projectionMatrixUniformLocation;

        /// <summary>
        /// Get the locations of uniform variables
        /// </summary>
        protected virtual void LoadUniformVariableLocations()
        {
            viewMatrixUniformLocation = glGetUniformLocation(programUint, "viewMatrix");
            projectionMatrixUniformLocation = glGetUniformLocation(programUint, "projectionMatrix");
        }

        /// <summary>
        /// Bind the uniform variables pre-render
        /// </summary>
        protected unsafe virtual void BindUniformVariables(ICamera camera)
        {
            //Bind the main camera
            glUniformMatrix4fv(viewMatrixUniformLocation, 1, false, camera.GetViewMatrix(1920, 1080).GetPointer());
            glUniformMatrix4fv(projectionMatrixUniformLocation, 1, false, camera.GetProjectionMatrix(1920, 1080).GetPointer());
        }

        /// <summary>
        /// Adds a batch element to the render cache
        /// </summary>
        protected void AddToBatch(ISharedRenderAttributes sharedRenderAttributes, IBatchElement<TBatch> batchElement)
        {
            if (RenderCache.ContainsKey(sharedRenderAttributes))
            {
                RenderCache[sharedRenderAttributes].Add(batchElement);
            }
            else
            {
                TBatch createdBatch = new TBatch();
                createdBatch.Add(batchElement);
                RenderCache.Add(sharedRenderAttributes, createdBatch);
            }
        }

        protected void RemoveFromBatch(ISharedRenderAttributes sharedRenderAttributes, IBatchElement<TBatch> batchElement)
        {
            RenderCache[sharedRenderAttributes].Remove(batchElement);
            if (RenderCache[sharedRenderAttributes].Count == 0)
                RenderCache.Remove(sharedRenderAttributes);
        }

        /// <summary>
        /// Generates the buffer locations for custom buffers
        /// </summary>
        /// <param name="batch"></param>
        private unsafe void GenerateCustomBufferLocations(TBatch batch)
        {
            //Generate buffer locations
            if (storedBufferLocations == null)
            {
                storedBufferLocations = new uint[batch.BatchVectorSizes.Length];
                for (int i = 0; i < batch.BatchVectorSizes.Length; i++)
                {
                    //Generate a buffer
                    storedBufferLocations[i] = glGenBuffer();
                    //Bind the buffer (so we can perform operations on it)
                    glBindBuffer(GL_ARRAY_BUFFER, storedBufferLocations[i]);
                    //Reserve space in the buffer
                    glBufferData(GL_ARRAY_BUFFER, sizeof(float) * batch.BatchVectorSizes[i] * batch.BatchSize, NULL, GL_STREAM_DRAW);
                }
            }
        }

        /// <summary>
        /// Populates the buffer data for a specified batch group
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="batchIndex"></param>
        /// <param name="count"></param>
        protected unsafe void BindBatchGroup(TBatch batch, int batchIndex, int count)
        {
            //Load the buffers
            for (int i = 0; i < batch.BatchVectorSizes.Length; i++)
            {
                fixed (float* bufferPointer = batch.GetArray(batchIndex, i))
                {
                    //Bind the buffer to perform operation on it
                    glBindBuffer(GL_ARRAY_BUFFER, storedBufferLocations[i]);
                    //Put data into the buffer
                    glBufferData(GL_ARRAY_BUFFER, sizeof(float) * batch.BatchVectorSizes[i] * batch.BatchSize, NULL, GL_STREAM_DRAW);
                    glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * batch.BatchVectorSizes[i] * count, bufferPointer);
                }
            }
        }

        /// <summary>
        /// Binds the atrib array at index provided, with the buffer data provided.
        /// </summary>
        protected unsafe void BindAttribArray(uint index, uint buffer, int size)
        {
            glEnableVertexAttribArray(index);
            glBindBuffer(GL_ARRAY_BUFFER, buffer);
            glVertexAttribPointer(
                index,              //Attribute - Where the layout location is in the vertex shader
                size,               //Size of the triangles (3 sides)
                GL_FLOAT,           //Type (Floats)
                false,              //Normalized (nope)
                0,                  //Stride (0)
                NULL                //Array buffer offset (null)
            );
        }

    }
}
