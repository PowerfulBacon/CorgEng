﻿using CorgEng.Constants;
using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.InputHandler;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.World;
using CorgEng.InputHandling.Events;
using CorgEng.UtilityTypes.Matrices;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.ClickHandler
{

    public class SelectableSystem : EntitySystem
    {

        [UsingDependency]
        private static IEntityPositionTracker World = default!;

        [UsingDependency]
        private static IIconFactory IconFactory = default!;

        [UsingDependency]
        private static IInputHandler InputHandler = default!;

        //Track the last clicked location for cyclical selection of entities
        private static int lastClickedX = 0;
        private static int lastClickedY = 0;
        private static IEnumerator<IWorldTrackComponent> contentEnumerator;

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.CLIENT_SYSTEM;

        private static IIcon selectorOverlay;
        private static SelectedComponent currentSelectedComponent;
        public static IEntity SelectedEntity { get; private set; }

        public override void SystemSetup(IWorld world)
        {
            selectorOverlay = IconFactory.CreateIcon("selector", 100, 1);
            RegisterLocalEvent<SelectedComponent, ComponentAddedEvent>(OnComponentAdded);
            RegisterLocalEvent<SelectedComponent, ComponentRemovedEvent>(OnComponentRemoved);
        }

        private void OnComponentAdded(IEntity entity, SelectedComponent selectedComponent, ComponentAddedEvent componentAddedEvent)
        {
            if (componentAddedEvent.Component != selectedComponent)
                return;
            if (SelectedEntity == entity)
                return;
            //Unselect the existing entity
            if (SelectedEntity != null)
            {
                SelectedEntity.RemoveComponent(currentSelectedComponent, false);
            }
            //Select this one
            SelectedEntity = entity;
            currentSelectedComponent = selectedComponent;
            //Call selection event
            new EntitySelectedEvent().Raise(entity);
            //Add an overlay
            new AddOverlayEvent(selectorOverlay).Raise(entity);
        }

        private void OnComponentRemoved(IEntity entity, SelectedComponent selectedComponent, ComponentRemovedEvent componentRemovedEvent)
        {
            //Not the correct component removed
            if (componentRemovedEvent.Component != selectedComponent)
                return;
            new EntityDeselectedEvent().Raise(entity);
            new RemoveOverlayEvent(selectorOverlay).Raise(entity);
            //Deselect
            if (SelectedEntity == entity)
            {
                SelectedEntity = null;
                currentSelectedComponent = null;
            }
        }

        /// <summary>
        /// Handle the situation where the player clicks, but
        /// the click isn't intercepted by anything.
        /// This means that the click should fall through to the world
        /// to see if anything in the world was clicked.
        /// 
        /// This is called directly without going through the signal handler
        /// as it has to have other signals intercept it first.
        /// </summary>
        public static void HandleWorldClick(MouseReleaseEvent releaseEvent, float windowWidth, float windowHeight)
        {
            if (SelectedEntity != null)
            {
                SelectedEntity.RemoveComponent(currentSelectedComponent, false);
            }
            //Transform the clicked position into world position
            IVector<float> clickedLocation = CorgEngMain.MainCamera.ScreenToWorldCoordinates(releaseEvent.CursorX * 2 - 1, releaseEvent.CursorY * 2 - 1, windowWidth, windowHeight);
            IVector<int> clickedGridLocation = World.GetGridPosition(clickedLocation);
            //Locate the world position that the mouse is currently over
            int clickedTileX = clickedGridLocation.X;
            int clickedTileY = clickedGridLocation.Y;
            //Determine our click mode
            //If we clicked on a different tile, reset the last contents index
            if (lastClickedX != clickedTileX || lastClickedY != clickedTileY || contentEnumerator == null)
            {
                contentEnumerator = World.GetContentsAt(SelectableComponent.TRACK_KEY, clickedTileX, clickedTileY, TemporaryMapLayers.DEFAULT_MAP_LAYER)?.GetContents().GetEnumerator();
                lastClickedX = clickedTileX;
                lastClickedY = clickedTileY;
            }
            if (contentEnumerator == null)
            {
                return;
            }
            //If we are on cycle, then only the tile matters.
            //For now we only support cycle, since its the only one I need
            //Cycle to the next element
            if (!contentEnumerator.MoveNext())
            {
                contentEnumerator.Reset();
                if (!contentEnumerator.MoveNext())
                {
                    return;
                }
            }
            //Select the element (TODO: SelectableComponent)
            contentEnumerator.Current.Parent.AddComponent(new SelectedComponent());
        }
    }
}
