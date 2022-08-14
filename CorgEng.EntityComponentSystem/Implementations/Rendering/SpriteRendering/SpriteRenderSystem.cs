﻿using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.Rendering;
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

        [UsingDependency]
        private static ISpriteRenderObjectFactory SpriteRenderObjectFactory;

        [UsingDependency]
        private static ILogger Log;

        //Runs only on the client, contains no server-side logic.
        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.CLIENT_SYSTEM | EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<SpriteRenderComponent, SetSpriteEvent>(OnSetSprite);
            RegisterLocalEvent<SpriteRenderComponent, SetSpriteRendererEvent>(OnSetRenderer);
            RegisterLocalEvent<SpriteRenderComponent, DeleteEntityEvent>(OnEntityDestroyed);
            RegisterLocalEvent<SpriteRenderComponent, ComponentAddedEvent>(OnComponentAdded);
            RegisterLocalEvent<SpriteRenderComponent, MoveEvent>(OnEntityMoved);
            RegisterLocalEvent<SpriteRenderComponent, InitialiseNetworkedEntityEvent>(OnInitialise);
            RegisterLocalEvent<SpriteRenderComponent, AddOverlayEvent>(AddOverlay);
            RegisterLocalEvent<SpriteRenderComponent, RemoveOverlayEvent>(RemoveOverlay);
        }

        private void OnInitialise(IEntity entity, SpriteRenderComponent spriteRenderComponent, InitialiseNetworkedEntityEvent componentAddedEvent)
        {
            if (!CorgEngMain.IsRendering)
                return;
            //Take the position
            spriteRenderComponent.CachedPosition = entity.GetComponent<TransformComponent>().Position.Copy();
            //Update the sprite
            UpdateSprite(spriteRenderComponent);
        }

        private void OnEntityDestroyed(IEntity entity, SpriteRenderComponent spriteRenderComponent, DeleteEntityEvent entityDeletedEvent)
        {
            //Stop rendering
            if (CorgEngMain.IsRendering && spriteRenderComponent.SpriteRenderer != null && spriteRenderComponent.SpriteRenderObject != null)
                spriteRenderComponent.SpriteRenderer.StopRendering(spriteRenderComponent.SpriteRenderObject);
            spriteRenderComponent.SpriteRendererIdentifier = 0;
            spriteRenderComponent.SpriteRenderObject = null;
        }

        public void OnComponentAdded(IEntity entity, SpriteRenderComponent spriteRenderComponent, ComponentAddedEvent componentAddedEvent)
        {
            //If the client isn't running, abort
            if (!CorgEngMain.IsRendering || spriteRenderComponent.Sprite == null)
                return;
            UpdateSprite(spriteRenderComponent);
        }

        /// <summary>
        /// Called when the parent entity is moved.
        /// </summary>
        private void OnEntityMoved(IEntity entity, SpriteRenderComponent spriteRenderComponent, MoveEvent moveEvent)
        {
            if (!CorgEngMain.IsRendering)
                return;
            if (spriteRenderComponent.SpriteRenderObject == null)
            {
                spriteRenderComponent.CachedPosition = moveEvent.NewPosition.Copy();
            }
            else
            {
                spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[3, 1] = moveEvent.NewPosition.X;
                spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[3, 2] = moveEvent.NewPosition.Y;
            }
        }

        private void OnSetRenderer(IEntity entity, SpriteRenderComponent spriteRenderComponent, SetSpriteRendererEvent setSpriteRenderer)
        {
            //If we are being rendered, stop being rendered
            if (CorgEngMain.IsRendering && spriteRenderComponent.SpriteRenderer != null && spriteRenderComponent.SpriteRenderObject != null)
                spriteRenderComponent.SpriteRenderer.StopRendering(spriteRenderComponent.SpriteRenderObject);
            //Set the sprite renderer
            spriteRenderComponent.SpriteRendererIdentifier = setSpriteRenderer.Target;
            //Start rendering again
            if (CorgEngMain.IsRendering && spriteRenderComponent.SpriteRenderer != null && spriteRenderComponent.SpriteRenderObject != null)
                spriteRenderComponent.SpriteRenderer.StartRendering(spriteRenderComponent.SpriteRenderObject);
        }

        private void OnSetSprite(IEntity entity, SpriteRenderComponent spriteRenderComponent, SetSpriteEvent setSpriteEvent)
        {
            //Store the sprite being used
            spriteRenderComponent.Sprite = setSpriteEvent.TextureFile;
            //If the client isn't running, abort
            if (!CorgEngMain.IsRendering)
                return;
            UpdateSprite(spriteRenderComponent);
        }

        private void UpdateSprite(SpriteRenderComponent spriteRenderComponent)
        {
            //Update the sprite data
            ITextureState newTexture = TextureFactory.GetTextureFromIconState(spriteRenderComponent.Sprite);
            if (spriteRenderComponent.SpriteRenderObject != null)
            {
                spriteRenderComponent.SpriteRenderObject.TextureFile.Value = newTexture.TextureFile.TextureID;
                spriteRenderComponent.SpriteRenderObject.TextureFileX.Value = newTexture.OffsetX;
                spriteRenderComponent.SpriteRenderObject.TextureFileY.Value = newTexture.OffsetY;
                spriteRenderComponent.SpriteRenderObject.TextureFileWidth.Value = newTexture.OffsetWidth;
                spriteRenderComponent.SpriteRenderObject.TextureFileHeight.Value = newTexture.OffsetHeight;
            }
            else
            {
                //Create teh sprite render boject
                spriteRenderComponent.SpriteRenderObject = SpriteRenderObjectFactory.CreateSpriteRenderObject(
                    newTexture.TextureFile.TextureID,
                    newTexture.OffsetX,
                    newTexture.OffsetY,
                    newTexture.OffsetWidth,
                    newTexture.OffsetHeight);
                if (spriteRenderComponent.CachedPosition != null)
                {
                    spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[3, 1] = spriteRenderComponent.CachedPosition.X;
                    spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[3, 2] = spriteRenderComponent.CachedPosition.Y;
                    spriteRenderComponent.CachedPosition = null;
                }
                //Start rendering
                if (spriteRenderComponent.SpriteRenderer != null)
                    spriteRenderComponent.SpriteRenderer.StartRendering(spriteRenderComponent.SpriteRenderObject);
            }
        }

        /// <summary>
        /// Adds an overlay to this sprite
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="spriteRenderComponent"></param>
        /// <param name="addOverlayEvent"></param>
        private void AddOverlay(IEntity entity, SpriteRenderComponent spriteRenderComponent, AddOverlayEvent addOverlayEvent)
        {
            spriteRenderComponent.SpriteRenderObject.AddOverlay(addOverlayEvent.TextureFile);
        }

        /// <summary>
        /// Remove an overlay from this sprite
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="spriteRenderComponent"></param>
        /// <param name="removeOverlayEvent"></param>
        private void RemoveOverlay(IEntity entity, SpriteRenderComponent spriteRenderComponent, RemoveOverlayEvent removeOverlayEvent)
        {
            spriteRenderComponent.SpriteRenderObject.RemoveOverlay(removeOverlayEvent.TextureFile);
        }

    }
}
