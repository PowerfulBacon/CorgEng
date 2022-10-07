using CorgEng.DependencyInjection.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Entities
{
    [Dependency]
    internal class EntityFactory : IEntityFactory
    {

        public IEntity CreateEmptyEntity(Action<IEntity> preInitialisationEvents)
        {
            //Create the blank entity.
            IEntity createdEntity = new Entity();
            //Run any events that need to happen before initialisation
            preInitialisationEvents?.Invoke(createdEntity);
            //Raise the initialise event
            new InitialiseEvent().Raise(createdEntity, true);
            //Return the entity that was created
            return createdEntity;
        }

        public IEntity CreateUninitialisedEntity()
        {
            return new Entity();
        }
    }
}
