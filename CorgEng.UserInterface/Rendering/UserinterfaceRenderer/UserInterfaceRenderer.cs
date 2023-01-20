using CorgEng.Constants;
using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Models;
using CorgEng.GenericInterfaces.Rendering.Renderers;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using CorgEng.GenericInterfaces.UserInterface.Rendering.Renderer;
using CorgEng.GenericInterfaces.UserInterface.Rendering.RenderObject;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using CorgEng.Rendering;
using CorgEng.Rendering.Exceptions;
using CorgEng.UtilityTypes.Batches;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer
{

    internal static class UserInterfaceRendererDependencyHolder
    {

        [UsingDependency]
        internal static IShaderFactory ShaderFactory;

        [UsingDependency]
        internal static ISquareModelFactory SquareModelFactory;

    }


    /// <summary>
    /// A basic interface renderer.
    /// Due to the nature of user interface components, each one
    /// will have a lot of different variables that need to be taken into account
    /// when rendering.
    /// As such, we will not use instancing but will just load standard uniform
    /// variables for rendering.
    /// </summary>
    internal abstract class UserInterfaceRenderer<IRenderObjectType> : IUserInterfaceRenderer<IRenderObjectType>
        where IRenderObjectType : IUserInterfaceRenderObject
    {

        protected static ISquareModelFactory SquareModelFactory
        {
            get => UserInterfaceRendererDependencyHolder.SquareModelFactory;
        }

        protected static IShaderFactory ShaderFactory {
            get => UserInterfaceRendererDependencyHolder.ShaderFactory;
        }

        //Only render UI components locally
        public uint NetworkIdentifier => RenderingConstants.NETWORK_RENDERING_ID_LOCAL;

        //The things we are rendering
        private ConcurrentDictionary<IRenderObjectType, bool> renderSet = new ConcurrentDictionary<IRenderObjectType, bool>();

        //The uint for the render program
        protected uint programUint;

        //The shaders that this renderer use
        protected abstract IShaderSet ShaderSet { get; }

        //The model
        private IModel model;

        private int UniformLocationPixelWidth;

        private int UniformLocationPixelHeight;

        public void Initialize()
        {
            //Create the model
            model = SquareModelFactory.CreateModel();
            //Create a program for the renderer
            programUint = glCreateProgram();
            //Setup the program and start using it
            glUseProgram(programUint);
            //Attach the shader set to the prorgam
            ShaderSet.AttachShaders(programUint);
            //Link the program
            glLinkProgram(programUint);
            //Fetch uniform variable locations
            FetchUniformVariableLocations();
            //Bind this
            UniformLocationPixelWidth = glGetUniformLocation(programUint, "pixelWidth");
            UniformLocationPixelHeight = glGetUniformLocation(programUint, "pixelHeight");
        }

        protected abstract void FetchUniformVariableLocations();

        protected abstract void BindUniformLocations(IRenderObjectType renderObject);

        /// <summary>
        /// Extrememly basic render that individually binds every objects uniform variables before rendering it.
        /// Assumes that there will never be a large amount being rendered on this.
        /// </summary>
        /// <param name="camera"></param>
        public void Render(int pixelWidth, int pixelHeight)
        {
            //Start using our program
            //Shaders were loaded during init
            glUseProgram(programUint);
            //Bind the model
            BindAttribArray(0, model.VertexBuffer, 3);
            BindAttribArray(1, model.UvBuffer, 2);
            //Set the attrib divisors
            glVertexAttribDivisor(0, 0);
            glVertexAttribDivisor(1, 0);

            glUniform1i(UniformLocationPixelWidth, pixelWidth);
            glUniform1i(UniformLocationPixelHeight, pixelHeight);

            foreach (IRenderObjectType renderObjectType in renderSet.Keys)
            {
                //Bind uniform locations
                BindUniformLocations(renderObjectType);
                //Do the actual render
                glDrawArrays(GL_TRIANGLES, 0, model.VertexCount);
            }

            //Disable vertex arrays
            glDisableVertexAttribArray(0);
            glDisableVertexAttribArray(1);
        }

        public void StartRendering(IRenderObjectType userInterfaceRenderObject)
        {
            renderSet.TryAdd(userInterfaceRenderObject, true);
        }

        public void StopRendering(IRenderObjectType userInterfaceRenderObject)
        {
            renderSet.Remove(userInterfaceRenderObject, out _);
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
