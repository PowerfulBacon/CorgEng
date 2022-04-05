using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.ContentLoading
{
    [Dependency(defaultDependency = true)]
    public class EntityCreator
    {

        /// <summary>
        /// Create an entity from the xml construct at a specified position.
        /// </summary>
        /// <param name="name">The name of the entity's xml data.</param>
        /// <param name="position">The position to spawn the entity at.</param>
        public T CreateEntity<T>(string name, IVector<float> position)
        {
            return (T)EntityConfig.LoadedEntityDefs[name].InstantiateAt(position);
        }
    }
}
