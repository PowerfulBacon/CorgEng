using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.World
{
    public interface IContentsHolder
    {

        /// <summary>
        /// Insert an element into this tile
        /// </summary>
        /// <param name="entity"></param>
        void Insert(IEntity entity);

        /// <summary>
        /// Remove an element from the world tile
        /// </summary>
        /// <param name="entity"></param>
        void Remove(IEntity entity);

        /// <summary>
        /// Get all the contents of this world tile
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEntity> GetContents();

    }
}
