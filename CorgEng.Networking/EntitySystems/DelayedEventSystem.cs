using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.Networking.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.EntitySystems
{
    internal class DelayedEventSystem : EntitySystem
    {

        internal static ConcurrentDictionary<uint, List<Action<IEntity>>> delayedEvents = new ConcurrentDictionary<uint, List<Action<IEntity>>>();

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM | EntitySystemFlags.CLIENT_SYSTEM;

        internal static void AddDelayedEvent(uint identifier, Action<IEntity> callbackAction)
        {
            if (!delayedEvents.TryAdd(identifier, new List<Action<IEntity>>() { callbackAction }))
            {
                delayedEvents[identifier].Add(callbackAction);
            }
        }

        public override void SystemSetup()
        {
            RegisterLocalEvent<NetworkTransformComponent, InitialiseNetworkedEntityEvent>((entity, component, signal) => {
                List<Action<IEntity>> actionsToPerform;
                if (delayedEvents.TryGetValue(entity.Identifier, out actionsToPerform))
                {
                    actionsToPerform.ForEach(f => f(entity));
                    delayedEvents.TryRemove(entity.Identifier, out _);
                }
            });
        }
    }
}
