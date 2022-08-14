using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.ContentLoading
{
    [Dependency]
    internal class EntityCreator : IEntityCreator
    {

        /// <summary>
        /// The entity nodes by the name
        /// </summary>
        public static Dictionary<string, EntityNode> EntityNodesByName = new Dictionary<string, EntityNode>();

        /// <summary>
        /// Create the entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEntity CreateEntity(string entityName)
        {
            if (EntityNodesByName.ContainsKey(entityName))
            {
                return EntityNodesByName[entityName].CreateEntity();
            }
            //Entity not found :(
            throw new Exception($"The entity with name {entityName} could not be spawned as it doesn't exist.");
        }

    }
}
