using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering
{
    public class SpriteRenderSystem : EntitySystem
    {

        [UsingDependency]
        private static ITextureFactory TextureFactory;

        public override void SystemSetup()
        {
            RegisterLocalEvent<SpriteRenderComponent, SetSpriteEvent>(OnSetSprite);
            RegisterLocalEvent<SpriteRenderComponent, SetSpriteRendererEvent>(OnSetRenderer);
        }

        private void OnSetRenderer(Entity entity, SpriteRenderComponent spriteRenderComponent, SetSpriteRendererEvent setSpriteRenderer)
        {
            //If we are being rendered, stop being rendered
            if (spriteRenderComponent.SpriteRenderer != null && spriteRenderComponent.SpriteRenderObject != null)
                spriteRenderComponent.SpriteRenderer.StopRendering(spriteRenderComponent.SpriteRenderObject);
            //Set the sprite renderer
            spriteRenderComponent.SpriteRenderer = setSpriteRenderer.Target;
            //Start rendering again
            if (spriteRenderComponent.SpriteRenderer != null && spriteRenderComponent.SpriteRenderObject != null)
                spriteRenderComponent.SpriteRenderer.StartRendering(spriteRenderComponent.SpriteRenderObject);
        }

        private void OnSetSprite(Entity entity, SpriteRenderComponent spriteRenderComponent, SetSpriteEvent setSpriteEvent)
        {
            //Store the sprite being used
            spriteRenderComponent.Sprite = setSpriteEvent.TextureFile;
            //Update the sprite data
            ITexture newTexture = TextureFactory.GetTextureFromIconState(spriteRenderComponent.Sprite);
            spriteRenderComponent.SpriteRenderObject.TextureFile.Value = newTexture.TextureID;
        }

    }
}
