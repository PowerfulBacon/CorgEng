using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.WorldManager
{
    [Dependency]
    internal class WorldFactory : IWorldFactory
    {

        public IWorld CreateWorld()
        {
            return new World();
        }

    }
}
