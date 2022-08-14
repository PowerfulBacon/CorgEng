using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.ContentLoading
{
    public interface IEntityCreator
    {

        /// <summary>
        /// Create an entity with the specific one
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        IEntity CreateEntity(string entityName);

    }
}
