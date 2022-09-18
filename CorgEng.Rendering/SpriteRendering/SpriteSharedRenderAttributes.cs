using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Models;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering
{
    internal sealed class SpriteSharedRenderAttributes : ISpriteSharedRenderAttributes
    {

        [UsingDependency]
        private static ISquareModelFactory squareModelFactory;

        private static IModel squareModel;

        /// <summary>
        /// The uint ID of the texture that this sprite uses.
        /// </summary>
        public uint SpriteTextureUint { get; set; }

        /// <summary>
        /// The layer to be rendered on
        /// </summary>
        //public float Layer { get; set; }

        public int VertexCount => 6;

        public IModel Model => squareModel;

        public SpriteSharedRenderAttributes(uint spriteTextureUint/*, float layer*/)
        {
            SpriteTextureUint = spriteTextureUint;
            //Layer = layer;
            if (squareModel == null)
                squareModel = squareModelFactory.CreateModel();
        }

        // ======================
        // THESE ARE USED FOR DICTIONARY HASHING
        // KEEP THEM UP TO DATE WITH THE UNIQUE ELEMENTS OF THE SHARED RENDER ATTRIBUTES
        // ======================

        public override bool Equals(object obj)
        {
            return obj is SpriteSharedRenderAttributes attributes &&
                   SpriteTextureUint == attributes.SpriteTextureUint; // &&
                   //Layer == attributes.Layer;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpriteTextureUint/*, Layer*/);
        }

    }
}
