using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
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

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// The entity nodes by the name
        /// </summary>
        public static Dictionary<string, IEntityDefinition> EntityNodesByName = new Dictionary<string, IEntityDefinition>();

        public Dictionary<string, IEntityDefinition> EntityNodes => EntityNodesByName;

        /// <summary>
        /// Create the entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEntity CreateEntity(string entityName)
        {
            try
            {
                if (EntityNodesByName.ContainsKey(entityName))
                {
                    IEntity createdEntity = EntityNodesByName[entityName].CreateEntity();
                    createdEntity.DefinitionName = entityName;
                    return createdEntity;
                }
                //Entity not found :(
                throw new Exception($"The entity with name {entityName} could not be spawned as it doesn't exist.");
            }
            catch (Exception e)
            {
                Logger.WriteLine($"An expection occured while creating entity with ID '{entityName}': {e}", LogType.ERROR);
                throw;
            }
        }

        /// <summary>
        /// Retrieve an object from the content loading system.
        /// </summary>
        /// <param name="objectIdentifier"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public object CreateObject(string objectIdentifier)
        {
            try
            {
                if (EntityLoader.LoadedDefinitions.ContainsKey(objectIdentifier))
                {
                    return EntityLoader.LoadedDefinitions[objectIdentifier].CreateInstance(null, new Dictionary<string, object>());
                }
                //Entity not found :(
                throw new Exception($"The object with name {objectIdentifier} could not be spawned as it doesn't exist.");
            }
            catch (Exception e)
            {
                Logger.WriteLine($"An expection occured while creating object with ID '{objectIdentifier}': {e}", LogType.ERROR);
                throw;
            }
        }

    }
}
