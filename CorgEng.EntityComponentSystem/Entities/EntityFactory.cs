using CorgEng.DependencyInjection.Dependencies;
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

        public IEntity CreateEmptyEntity()
        {
            return new Entity();
        }

    }
}
