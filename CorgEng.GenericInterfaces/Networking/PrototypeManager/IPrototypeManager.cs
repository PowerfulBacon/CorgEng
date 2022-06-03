using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.PrototypeManager
{
    public interface IPrototypeManager
    {

        /// <summary>
        /// Gets the prototype relating to a specific entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IPrototype GetPrototype(IEntity entity);

    }
}
