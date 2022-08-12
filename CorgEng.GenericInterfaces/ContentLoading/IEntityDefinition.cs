using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.ContentLoading
{
    public interface IEntityDefinition
    {

        /// <summary>
        /// Create the entity
        /// </summary>
        /// <returns></returns>
        IEntity CreateEntity();

    }
}
