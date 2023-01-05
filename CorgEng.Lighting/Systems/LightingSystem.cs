using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.Lighting.Components;
using CorgEng.Lighting.RenderCores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Lighting.Systems
{
    internal class LightingSystem : EntitySystem
    {

        public override EntitySystemFlags SystemFlags => throw new NotImplementedException();

        public override void SystemSetup()
        {
            //When the lighting object initialises, set it to start rendering on the specified system
            RegisterLocalEvent<LightingComponent, InitialiseEvent>((entity, lightingComponent, signal) => {
                new SetSpriteRendererEvent(LightingRenderCore.Singleton.lightRenderer).Raise(entity);
            });
        }

    }
}
