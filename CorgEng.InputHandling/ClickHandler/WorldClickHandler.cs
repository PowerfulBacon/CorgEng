﻿using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
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

    internal class WorldClickHandler : EntitySystem
    {

        [UsingDependency]
        private static ILogger Logger;

        [UsingDependency]
        private static IWorld World;

        //Track the last clicked location for cyclical selection of entities
        private static int lastClickedX = 0;
        private static int lastClickedY = 0;
        private static IEnumerator<IEntity> contentEnumerator;

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.CLIENT_SYSTEM;

        private SelectedComponent currentSelectedComponent;
        private IEntity selectedEntity;

        public override void SystemSetup()
        {
            RegisterLocalEvent<SelectedComponent, ComponentAddedEvent>(OnComponentAdded);
            RegisterLocalEvent<SelectedComponent, ComponentRemovedEvent>(OnComponentRemoved);
        }

        private void OnComponentAdded(IEntity entity, SelectedComponent selectedComponent, ComponentAddedEvent componentAddedEvent)
        {
            if (componentAddedEvent.Component != selectedComponent)
                return;
            //Unselect the existing entity
            if (selectedEntity != null)
            {
                selectedEntity.RemoveComponent(currentSelectedComponent, false);
            }
            //Select this one
            selectedEntity = entity;
            currentSelectedComponent = selectedComponent;
        }

        private void OnComponentRemoved(IEntity entity, SelectedComponent selectedComponent, ComponentRemovedEvent componentRemovedEvent)
        {
            //Not the correct component removed
            if (componentRemovedEvent.Component != selectedComponent)
                return;
            //Deselect
            if (selectedEntity == entity)
            {
                selectedEntity = null;
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
            //Transform the clicked position into world position
            IVector<float> clickedLocation = CorgEngMain.MainCamera.ScreenToWorldCoordinates(releaseEvent.CursorX * 2 - 1, releaseEvent.CursorY * 2 - 1, windowWidth, windowHeight);
            //Locate the world position that the mouse is currently over
            int clickedTileX = (int)Math.Round(clickedLocation.X);
            int clickedTileY = (int)Math.Round(clickedLocation.Y);
            //Check what we actually pressed on
            //Determine our click mode
            //If we clicked on a different tile, reset the last contents index
            if (lastClickedX != clickedTileX || lastClickedY != clickedTileY || contentEnumerator == null)
            {
                contentEnumerator = World.GetContentsAt(clickedTileX, clickedTileY, 0)?.GetContents().GetEnumerator();
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
            //Select the element
            contentEnumerator.Current.AddComponent(new SelectedComponent());
            Logger.WriteLine($"({releaseEvent.CursorX * 2 - 1}, {releaseEvent.CursorY * 2 - 1}), {clickedLocation} Selected Entity: {contentEnumerator.Current}");
        }
    }
}