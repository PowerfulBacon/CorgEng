using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Entities
{
    public static class EntityManager
    {

        [UsingDependency]
        private static ILogger Logger;

        private static Entity[] entityList = new Entity[1024];

        public static void RegisterEntity(Entity entity)
        {
            while (entityList.Length < entity.Identifier)
            {
                Entity[] newEntityList = new Entity[entityList.Length * 2];
                entityList.CopyTo(newEntityList, 0);
                entityList = newEntityList;
                Logger?.WriteLine($"Extended the entity list to {entityList.Length} entities.", LogType.DEBUG);
            }
            entityList[entity.Identifier] = entity;
        }

        public static void RemoveEntity(Entity entity)
        {
            entityList[entity.Identifier] = null;
        }

        public static Entity GetEntity(int identifier)
        {
            return entityList[identifier];
        }

    }
}
