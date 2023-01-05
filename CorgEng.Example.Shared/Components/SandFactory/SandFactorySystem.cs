using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.Example.Shared.Components.Gravity;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.Lighting.Components;
using CorgEng.Networking.Components;
using CorgEng.Pathfinding.Components;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example.Shared.Components.SandFactory
{
    internal class SandFactorySystem : ProcessingSystem
    {

        [UsingDependency]
        private static IEntityFactory EntityFactory = default!;

        [UsingDependency]
        private static IIconFactory IconFactory = default!;

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

        protected override int ProcessDelay => 50;

        public override void SystemSetup()
        {
            RegisterLocalEvent<SandFactoryComponent, InitialiseEvent>((entity, component, signal) => {
                TransformComponent transform = entity.GetComponent<TransformComponent>();
                RegisterProcess<SandFactoryComponent>(entity, (entity, component, deltaTime) => {
                    EntityFactory.CreateEmptyEntity(entity => {
                        //Add components
                        entity.AddComponent(new NetworkTransformComponent());
                        entity.AddComponent(new SpriteRenderComponent());
                        entity.AddComponent(new SandComponent());
                        entity.AddComponent(new SolidComponent());
                        //Update the entity
                        new SetPositionEvent(new Vector<float>(transform.Position.X, transform.Position.Y)).Raise(entity);
                        new SetSpriteEvent(IconFactory.CreateIcon("sand", 5)).Raise(entity);
                        new SetSpriteRendererEvent(1).Raise(entity);
                    });
                });
            });
        }

    }
}
