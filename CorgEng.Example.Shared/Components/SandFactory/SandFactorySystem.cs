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
using CorgEng.UtilityTypes.Colours;
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
        private static IIconFactory IconFactory = default!;

        private static Random DebugRandom = new Random();

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

        protected override int ProcessDelay => 2000;

        public override void SystemSetup(IWorld world)
        {
            RegisterLocalEvent<SandFactoryComponent, InitialiseEvent>((entity, component, signal) => {
                TransformComponent transform = entity.GetComponent<TransformComponent>();
                RegisterProcess<SandFactoryComponent>(entity, (entity, component, deltaTime) => {
                    world.EntityManager.CreateEmptyEntity(entity => {
                        //Add components
                        entity.AddComponent(new NetworkTransformComponent());
                        entity.AddComponent(new SpriteRenderComponent());
                        entity.AddComponent(new SandComponent());
                        entity.AddComponent(new SolidComponent());
                        entity.AddComponent(new LightingComponent() {
                            Colour = new Colour(DebugRandom.NextSingle(), DebugRandom.NextSingle(), DebugRandom.NextSingle(), DebugRandom.NextSingle()),
                        });
                        //Update the entity
                        new SetPositionEvent(new Vector<float>(transform.Position.Value.X, transform.Position.Value.Y)).Raise(entity);
                        new SetSpriteEvent(IconFactory.CreateIcon("sand", 5, Constants.RenderingConstants.DEFAULT_RENDERER_PLANE)).Raise(entity);
                        new SetSpriteRendererEvent(1).Raise(entity);
                    });
                });
            });
        }

    }
}
