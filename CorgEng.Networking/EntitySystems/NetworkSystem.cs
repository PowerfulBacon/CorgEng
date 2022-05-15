using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.EntitySystems
{
    internal class NetworkSystem : EntitySystem
    {

        public override void SystemSetup()
        {
            RegisterLocalEvent<TransformComponent, NetworkedEventRaisedEvent>(OnNetworkedEventRaised);
        }

        /// <summary>
        /// Triggered when a networked event was raised.
        /// Uses the transform component to determine if the entity is near a camera,
        /// so any networked events need to has a transform component on the entity.
        /// </summary>
        private void OnNetworkedEventRaised(Entity entity, TransformComponent transformComponent, NetworkedEventRaisedEvent networkedEventRaisedEvent)
        {

        }

    }
}
