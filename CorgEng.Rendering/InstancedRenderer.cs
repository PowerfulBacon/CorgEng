using CorgEng.Core.Dependencies;
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

        protected abstract void LoadUniformVariableLocations();

        public void Render()
        {
            //Start using our program
            //Shaders were loaded during init
            glUseProgram(programUint);
            //Bind uniform variables
            BindUniformVariables();
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
                    //Rendering preperation
                    BindBatchAttributes((TSharedRenderAttributes)sharedRenderAttribute, renderBatch);
                    //Deal with rendering
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
                        glDrawArraysInstanced(GL_TRIANGLES, 0, sharedRenderAttribute.VertexCount, count);
                    }
                }
            }
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
        /// Bind the attribute arrays, set vertex attribute divisors
        /// and load texture information so that a target batch
        /// can be rendered.
        /// </summary>
        protected abstract void BindBatchAttributes(TSharedRenderAttributes sharedRenderAttributes, TBatch batch);

        /// <summary>
        /// Populates the buffer data for a specified batch group
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="batchIndex"></param>
        /// <param name="count"></param>
        protected unsafe void BindBatchGroup(TBatch batch, int batchIndex, int count)
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

        protected abstract void BindUniformVariables();

    }
}
