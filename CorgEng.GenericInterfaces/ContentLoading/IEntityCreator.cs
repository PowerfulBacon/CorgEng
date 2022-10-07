using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.ContentLoading
{
    public interface IEntityCreator
    {

        Dictionary<string, IEntityDefinition> EntityNodes { get; }

        /// <summary>
        /// Create an entity with the specific one
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns>Returns an uninitialised entity.</returns>
        IEntity CreateEntity(string entityName, Action<IEntity> preInitialisationEvents);

        /// <summary>
        /// Create an object with the specified identifier.
        /// </summary>
        /// <param name="objectIdentifier">The identifier of the object to create.</param>
        /// <returns>Returns an object created based on the definition file.</returns>
        object CreateObject(string objectIdentifier);

    }
}
