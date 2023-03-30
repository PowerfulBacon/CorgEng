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
    public class EntityManager : IEntityManager
    {

        [UsingDependency]
        private static ILogger Logger;

        private IEntity[] entityList = new IEntity[1024];

        internal int GarbageCollectionCount = 0;

        internal int DeletionCount = 0;

        /// <summary>
        /// Amount of created entities
        /// </summary>
        private uint CreatedEntityCount = 0;

        private IWorld world;

        public EntityManager(IWorld world)
        {
            this.world = world;
        }

        public void RegisterEntity(IEntity entity)
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

        public uint GetNewEntityId()
        {
            return CreatedEntityCount++;
        }

        public void RemoveEntity(IEntity entity)
        {
            entityList[entity.Identifier] = null;
        }

        public IEntity GetEntity(uint identifier)
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
        public void InternallyDelete(IEntity entity)
        {
            if ((entity.EntityFlags & EntityFlags.DESTROYED) != 0)
            {
                throw new Exception("Attempting to delete an already deleted entity.");
            }
            Interlocked.Increment(ref DeletionCount);
            RemoveEntity(entity);
            //Send component removed signals to all
            //Signals are sent synchronously, shouldn't slow down too much since deletion
            //is on its own thread.
            for (int i = entity.Components.Count - 1; i >= 0; i--)
            {
                new ComponentRemovedEvent(entity.Components[i]).Raise(entity, true);
            }
            //Remove all components
            for (int i = entity.Components.Count - 1; i >= 0; i--)
            {
                entity.RemoveComponent(entity.Components[i], false, true);
            }
            //Mark the entity as destroyed
            entity.EntityFlags |= EntityFlags.DESTROYED;
            //Logger.WriteLine($"Entity deletion triggered. {GarbageCollectionCount}/{DeletionCount}", LogType.TEMP);
        }

        public IEntity[] GetEntityArrayUnsafe()
        {
            return entityList;
        }

        public IEntity CreateEmptyEntity(Action<IEntity> preInitialisationEvents)
        {
            //Create the blank entity.
            IEntity createdEntity = new Entity(this);
            //Run any events that need to happen before initialisation
            preInitialisationEvents?.Invoke(createdEntity);
            //Raise the initialise event
            new InitialiseEvent().Raise(createdEntity, true);
            //Return the entity that was created
            return createdEntity;
        }

        public IEntity CreateUninitialisedEntity()
        {
            return new Entity(this);
        }

        public IEntity CreateUninitialisedEntity(uint entityIdentifier)
        {
            return new Entity(this, entityIdentifier);
        }
    }
}
