using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Initialisation
{
    internal class EntityInitialisationSystem : EntitySystem
    {

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM | EntitySystemFlags.CLIENT_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<DeleteableComponent, InitialiseEvent>(OnInitialisation);
        }

        private void OnInitialisation(IEntity entity, DeleteableComponent deletableComponent, InitialiseEvent initialiseEvent)
        {
            //Doing something extremely weird
            if ((entity.EntityFlags & EntityFlags.DESTROYED) != 0)
            {
                throw new Exception("Attempting to initialise a destroyed entity, something weird is being done.");
            }
            if ((entity.EntityFlags & EntityFlags.INITIALISED) != 0)
            {
                throw new Exception("Attempted to initialise an already initialised entity.");
            }
            //Toggle the flag on
            entity.EntityFlags |= EntityFlags.INITIALISED;
        }

    }
}
