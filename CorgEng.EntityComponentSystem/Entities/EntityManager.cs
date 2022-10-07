using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Entities
{
    public static class EntityManager
    {

        [UsingDependency]
        private static ILogger Logger;

        private static IEntity[] entityList = new IEntity[1024];

        internal static int GarbageCollectionCount = 0;

        internal static int DeletionCount = 0;

        /// <summary>
        /// Amount of created entities
        /// </summary>
        private static uint CreatedEntityCount = 0;

        public static void RegisterEntity(IEntity entity)
        {
            while (entityList.Length <= entity.Identifier)
            {
                IEntity[] newEntityList = new IEntity[entityList.Length * 2];
                entityList.CopyTo(newEntityList, 0);
                entityList = newEntityList;
                Logger?.WriteLine($"Extended the entity list to {entityList.Length} entities.", LogType.DEBUG);
            }
            entityList[entity.Identifier] = entity;
            //Raise a new entity created event
            new NewEntityEvent(entity.Identifier).RaiseGlobally();
        }

        public static uint GetNewEntityId()
        {
            return CreatedEntityCount++;
        }

        public static void RemoveEntity(IEntity entity)
        {
            entityList[entity.Identifier] = null;
        }

        public static IEntity GetEntity(uint identifier)
        {
            if (identifier < 0 || identifier >= entityList.Length)
                return null;
            return entityList[identifier];
        }

        /// <summary>
        /// Delete this entity, remove all references to it.
        /// Triggered when an EntityDeletedEvent is raised against an entity.
        /// 
        /// EntityDeletedEvent
        /// -> Networking
        /// -> Deletion System Raised
        /// -> Delete() method
        /// -> Component local removal + EntityManager removal
        /// </summary>
        internal static void Delete(this IEntity entity)
        {
            if ((entity.EntityFlags & EntityFlags.DESTROYED) != 0)
            {
                throw new Exception("Attempting to delete an already deleted entity.");
            }
            Interlocked.Increment(ref DeletionCount);
            RemoveEntity(entity);
            //Remove all components
            for (int i = entity.Components.Count - 1; i >= 0; i--)
            {
                entity.RemoveComponent(entity.Components[i], false);
            }
            //Mark the entity as destroyed
            entity.EntityFlags |= EntityFlags.DESTROYED;
            //Logger.WriteLine($"Entity deletion triggered. {GarbageCollectionCount}/{DeletionCount}", LogType.TEMP);
        }

    }
}
