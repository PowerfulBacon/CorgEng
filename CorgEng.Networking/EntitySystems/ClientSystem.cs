using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.Networking.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.EntitySystems
{
    /// <summary>
    /// Responsible for sending information about entities in view
    /// when a client moves.
    /// </summary>
    internal class ClientSystem : EntitySystem
    {

        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<ClientComponent, MoveEvent>(OnClientMoved);
        }

        /// <summary>
        /// Called when the client entity is moved.
        /// Will transmit information about new items in view to the client.
        /// Anything thats no longer in view will just be left idle in memory, and
        /// will be recompared against prototypes when it comes back into view.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="clientComponent"></param>
        /// <param name="moveEvent"></param>
        private void OnClientMoved(Entity entity, ClientComponent clientComponent, MoveEvent moveEvent)
        {
            //Calculate what should be added to view.
            //Send this information across to the client.
        }

    }
}
