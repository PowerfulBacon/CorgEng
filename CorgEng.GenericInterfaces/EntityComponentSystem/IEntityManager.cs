using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface IEntityManager
    {

        void RegisterEntity(IEntity entity);

        uint GetNewEntityId();

        void RemoveEntity(IEntity entity);

        void InternallyDelete(IEntity entity);

        IEntity GetEntity(uint identifier);

        IEntity[] GetEntityArrayUnsafe();

        IEntity CreateEmptyEntity(Action<IEntity> preInitialisationEvents);

        IEntity CreateUninitialisedEntity();

        IEntity CreateUninitialisedEntity(uint entityIdentifier);

    }
}
