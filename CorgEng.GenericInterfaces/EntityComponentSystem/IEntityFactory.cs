using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface IEntityFactory
    {

        /// <summary>
        /// Creates an empty entity with no components
        /// </summary>
        /// <returns></returns>
        IEntity CreateEmptyEntity(Action<IEntity> preInitialisationEvents);

        /// <summary>
        /// Creates an empty entity with no components
        /// </summary>
        /// <returns></returns>
        IEntity CreateUninitialisedEntity();

    }
}
