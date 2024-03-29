﻿using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering
{
    [Dependency]
    public sealed class SpriteRenderObjectFactory : ISpriteRenderObjectFactory
    {

        public ISpriteRenderObject CreateSpriteRenderObject(uint textureFile, float textureX, float textureY, float textureWidth, float textureHeight, float layer)
        {
            return new SpriteRenderObject(textureFile, textureX, textureY, textureWidth, textureHeight, layer);
        }

    }
}
