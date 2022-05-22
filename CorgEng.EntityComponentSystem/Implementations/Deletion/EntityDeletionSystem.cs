﻿using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Deletion
{
    internal class EntityDeletionSystem : EntitySystem
    {

        //Executes on the client and the server to delete
        //entities when requested.
        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM | EntitySystemFlags.CLIENT_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<DeleteableComponent, DeleteEntityEvent>(OnEntityDeleted);
        }

        private void OnEntityDeleted(Entity entity, DeleteableComponent deleteableComponent, DeleteEntityEvent entityDeletedEvent)
        {
            //Delete the entity
            entity.Delete();
        }

    }
}
