using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public class WorldObject
    {

        public IWorld world { get; }

        public WorldObject(IWorld world)
        {
            this.world = world;
        }

    }
}
