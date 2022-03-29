﻿using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering
{
    internal class SpriteSharedRenderAttributes : ISharedRenderAttributes
    {

        /// <summary>
        /// The uint ID of the texture that this sprite uses.
        /// </summary>
        public uint SpriteTextureUint { get; set; }
        
        public int VertexCount => 6;

        public SpriteSharedRenderAttributes(uint spriteTextureUint)
        {
            SpriteTextureUint = spriteTextureUint;
        }

        // ======================
        // THESE ARE USED FOR DICTIONARY HASHING
        // KEEP THEM UP TO DATE WITH THE UNIQUE ELEMENTS OF THE SHARED RENDER ATTRIBUTES
        // ======================

        public override bool Equals(object obj)
        {
            return obj is SpriteSharedRenderAttributes attributes &&
                   SpriteTextureUint == attributes.SpriteTextureUint &&
                   VertexCount == attributes.VertexCount;
        }

        public override int GetHashCode()
        {
            int hashCode = -1086999075;
            hashCode = hashCode * -1521134295 + SpriteTextureUint.GetHashCode();
            hashCode = hashCode * -1521134295 + VertexCount.GetHashCode();
            return hashCode;
        }

    }
}
