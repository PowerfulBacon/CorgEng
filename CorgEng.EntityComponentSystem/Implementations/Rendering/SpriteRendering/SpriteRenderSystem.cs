using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.UtilityTypes;
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
        private static ITextureOffsetCalculator TextureOffsetCalculator;

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
            //Component.GetTemplate<TransformComponent>().Position
            RegisterLocalEvent<SpriteRenderComponent, RotationEvent>(OnEntityRotated);
            RegisterLocalEvent<SpriteRenderComponent, InitialiseNetworkedEntityEvent>(OnInitialise);
            RegisterLocalEvent<SpriteRenderComponent, AddOverlayEvent>(AddOverlay);
            RegisterLocalEvent<SpriteRenderComponent, RemoveOverlayEvent>(RemoveOverlay);
            RegisterLocalEvent<SpriteRenderComponent, SetDirectionEvent>(OnSetIconDirection);
            RegisterLocalEvent<SpriteRenderComponent, ContentsChangedEvent>(OnContentsChanged);
        }

        #region Component Handling

        private void OnInitialise(IEntity entity, SpriteRenderComponent spriteRenderComponent, InitialiseNetworkedEntityEvent componentAddedEvent)
        {
            if (!CorgEngMain.IsRendering)
                return;
            //Take the position
            spriteRenderComponent.CachedPosition = entity.GetComponent<TransformComponent>().Position.Value.Copy();
            //Update the sprite
            UpdateSprite(spriteRenderComponent);
            //Apply initial overlays
            if (spriteRenderComponent.InitialOverlays != null)
            {
                foreach (IIcon overlay in spriteRenderComponent.InitialOverlays)
                {
                    spriteRenderComponent.SpriteRenderObject.AddOverlay(overlay);
                }
                spriteRenderComponent.InitialOverlays = null;
            }
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
            //Apply initial overlays
            if (spriteRenderComponent.InitialOverlays != null)
            {
                foreach (IIcon overlay in spriteRenderComponent.InitialOverlays)
                {
                    spriteRenderComponent.SpriteRenderObject.AddOverlay(overlay);
                }
                spriteRenderComponent.InitialOverlays = null;
            }
        }

        #endregion

        #region Transformations

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

        /// <summary>
        /// Called when
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="spriteRenderComponent"></param>
        /// <param name="rotationEvent"></param>
        private void OnEntityRotated(IEntity entity, SpriteRenderComponent spriteRenderComponent, RotationEvent rotationEvent)
        {
            if (!CorgEngMain.IsRendering)
                return;
            if (spriteRenderComponent.SpriteRenderObject == null)
            {
                spriteRenderComponent.CachedRotation = rotationEvent.NewRotation.Copy();
            }
            else
            {
                spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[1, 1] = (float)Math.Cos(rotationEvent.NewRotation[0]);
                spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[1, 2] = (float)-Math.Sin(rotationEvent.NewRotation[0]);
                spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[2, 1] = (float)Math.Sin(rotationEvent.NewRotation[0]);
                spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[2, 2] = (float)Math.Cos(rotationEvent.NewRotation[0]);
            }
        }

        #endregion

        #region Sprite & Render System

        private void OnSetRenderer(IEntity entity, SpriteRenderComponent spriteRenderComponent, SetSpriteRendererEvent setSpriteRenderer)
        {
            //If we are being rendered, stop being rendered
            if (CorgEngMain.IsRendering && spriteRenderComponent.SpriteRenderer != null && spriteRenderComponent.SpriteRenderObject != null && spriteRenderComponent.IsRendering)
                StopRendering(spriteRenderComponent);
            //Set the sprite renderer
            spriteRenderComponent.SpriteRendererIdentifier = setSpriteRenderer.Target;
            //Start rendering again
            if (CorgEngMain.IsRendering && spriteRenderComponent.SpriteRenderer != null && spriteRenderComponent.SpriteRenderObject != null && spriteRenderComponent.WantsToRender)
                StartRendering(spriteRenderComponent);
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

        private void StopRendering(SpriteRenderComponent spriteRenderComponent)
        {
            //Not rendering already
            if (!spriteRenderComponent.IsRendering)
                return;
            //Start rendering
            if (spriteRenderComponent.SpriteRenderer != null)
            {
                spriteRenderComponent.SpriteRenderer.StopRendering(spriteRenderComponent.SpriteRenderObject);
                spriteRenderComponent.IsRendering = false;
            }
        }

        private void StartRendering(SpriteRenderComponent spriteRenderComponent)
        {
            //Not rendering already
            if (spriteRenderComponent.IsRendering)
                return;
            //Start rendering
            if (spriteRenderComponent.SpriteRenderer != null)
            {
                spriteRenderComponent.SpriteRenderer.StartRendering(spriteRenderComponent.SpriteRenderObject);
                spriteRenderComponent.IsRendering = true;
            }
        }

        private void UpdateSprite(SpriteRenderComponent spriteRenderComponent)
        {
            //Update the sprite data
            ITextureState newTexture = TextureFactory.GetTextureFromIconState(spriteRenderComponent.Sprite);
            if (spriteRenderComponent.SpriteRenderObject != null)
            {
                IVector<float> offset = TextureOffsetCalculator.GetTextureOffset(newTexture, spriteRenderComponent.Sprite.DirectionalState);
                spriteRenderComponent.SpriteRenderObject.TextureFile.Value = newTexture.TextureFile.TextureID;
                spriteRenderComponent.SpriteRenderObject.TextureFileX.Value = offset.X;
                spriteRenderComponent.SpriteRenderObject.TextureFileY.Value = offset.Y;
                spriteRenderComponent.SpriteRenderObject.TextureFileWidth.Value = newTexture.OffsetWidth;
                spriteRenderComponent.SpriteRenderObject.TextureFileHeight.Value = newTexture.OffsetHeight;
            }
            else
            {
                IVector<float> offset = TextureOffsetCalculator.GetTextureOffset(newTexture, spriteRenderComponent.Sprite.DirectionalState);
                //Create teh sprite render boject
                spriteRenderComponent.SpriteRenderObject = SpriteRenderObjectFactory.CreateSpriteRenderObject(
                    newTexture.TextureFile.TextureID,
                    offset.X,
                    offset.Y,
                    newTexture.OffsetWidth,
                    newTexture.OffsetHeight,
                    spriteRenderComponent.Sprite.Layer
                    );
                //Attach the icon (Check this for icon changes, memory leaks etc.)
                spriteRenderComponent.Sprite.ValueChanged += () => {
                    spriteRenderComponent.SpriteRenderObject.RedrawFromIcon(spriteRenderComponent.Sprite);
                };
                //Set the colour
                spriteRenderComponent.SpriteRenderObject.Colour.Value = spriteRenderComponent.Sprite.Colour;
                //Set the layer
                spriteRenderComponent.SpriteRenderObject.IconLayer.Value[0] = spriteRenderComponent.Sprite.Layer;
                if (spriteRenderComponent.CachedPosition != null)
                {
                    spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[3, 1] = spriteRenderComponent.CachedPosition.X;
                    spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[3, 2] = spriteRenderComponent.CachedPosition.Y;
                    spriteRenderComponent.CachedPosition = null;
                }
                if (spriteRenderComponent.CachedRotation != null)
                {
                    spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[1, 1] = (float)Math.Cos(spriteRenderComponent.CachedRotation[0]);
                    spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[1, 2] = (float)-Math.Sin(spriteRenderComponent.CachedRotation[0]);
                    spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[2, 1] = (float)Math.Sin(spriteRenderComponent.CachedRotation[0]);
                    spriteRenderComponent.SpriteRenderObject.CombinedTransform.Value[2, 2] = (float)Math.Cos(spriteRenderComponent.CachedRotation[0]);
                    spriteRenderComponent.CachedRotation = null;
                }
                //Start rendering
                if (spriteRenderComponent.WantsToRender)
                    StartRendering(spriteRenderComponent);
            }
        }

        #endregion

        #region Direction

        /// <summary>
        /// Set the icon's direction
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="spriteRenderComponent"></param>
        /// <param name="setDirectionEvent"></param>
        private void OnSetIconDirection(IEntity entity, SpriteRenderComponent spriteRenderComponent, SetDirectionEvent setDirectionEvent)
        {
            spriteRenderComponent.Sprite.DirectionalState = setDirectionEvent.DirectionalState;
            UpdateSprite(spriteRenderComponent);
        }

        #endregion

        #region Overlay Handling

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

        #endregion

        #region Contents Handling

        /// <summary>
        /// Stop rendering if we enter the contents of something.
        /// Start rendering if we left the contents of something.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="spriteRenderComponent"></param>
        /// <param name="contentsChangedEvent"></param>
        private void OnContentsChanged(IEntity entity, SpriteRenderComponent spriteRenderComponent, ContentsChangedEvent contentsChangedEvent)
        {
            if (contentsChangedEvent.NewHolder != null)
            {
                spriteRenderComponent.WantsToRender = false;
                StopRendering(spriteRenderComponent);
            }
            else
            {
                spriteRenderComponent.WantsToRender = true;
                StartRendering(spriteRenderComponent);
            }
        }

        #endregion

    }
}
