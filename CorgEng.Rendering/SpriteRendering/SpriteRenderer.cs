using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering
{
    [Dependency(defaultDependency = true)]
    internal sealed class SpriteRenderer : InstancedRenderer<SpriteSharedRenderAttributes, SpriteBatch>, ISpriteRenderer
    {

        protected override IShaderSet ShaderSet => throw new NotImplementedException();

        public void StartRendering(ISpriteRenderObject spriteRenderObject)
        {
            throw new NotImplementedException();
        }

        public void StopRendering(ISpriteRenderObject spriteRenderObject)
        {
            throw new NotImplementedException();
        }

        protected override void BindBatchAttributes(SpriteSharedRenderAttributes sharedRenderAttributes, SpriteBatch batch)
        {
            throw new NotImplementedException();
        }

        protected override void BindUniformVariables()
        {
            throw new NotImplementedException();
        }

        protected override void LoadUniformVariableLocations()
        {
            throw new NotImplementedException();
        }
    }
}
