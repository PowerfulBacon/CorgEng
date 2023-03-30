using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.World
{
    [TestClass]
    internal class WorldEntitySystem
    {

        [UsingDependency]
        private static IWorldFactory WorldFactory = null!;

        

    }
}
