﻿using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.Lighting.Components;
using CorgEng.Lighting.RenderCores;
using CorgEng.UtilityTypes.Matrices;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Lighting.Systems
{
    internal class LightingSystem : EntitySystem
    {

        [UsingDependency]
        private static IIconFactory IconFactory = default!;

        [UsingDependency]
        private static IEntityCreator EntityCreator = default!;

        public override EntitySystemFlags SystemFlags => throw new NotImplementedException();

        public override void SystemSetup()
        {
            //When the lighting object initialises, set it to start rendering on the specified system
            RegisterLocalEvent<LightingComponent, InitialiseEvent>((entity, lightingComponent, signal) => {
                //Create an overlay lighting icon.
                IIcon createdIcon = IconFactory.CreateIcon("light_mask", 100, LightingRenderCore.LIGHTING_PLANE);
                createdIcon.Transform = new Matrix(new float[,]
                    {
                        { 8, 0, 0 },
                        { 0, 8, 0 },
                        { 0, 0, 1 },
                    });
                //Apply the overlay to the entity
                new AddOverlayEvent(createdIcon).Raise(entity);
            });
        }

    }
}
