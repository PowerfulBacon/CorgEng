﻿using CorgEng.ContentLoading;
using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.ContentLoadingTests
{
    [TestClass]
    public class ContentLoadingTests : TestBase
    {

        [UsingDependency]
        private static IEntityCreator EntityCreator;

        [UsingDependency]
        private static ILogger Logger;

        [UsingDependency]
        private static IWorldFactory WorldFactory;

        [TestMethod]
        public void TestContentLoading()
        {
            IWorld testWorld = WorldFactory.CreateWorld();
            ConsoleLogger.ExceptionCount = 0;
            EntityLoader.LoadEntities();
            Assert.AreEqual(0, ConsoleLogger.ExceptionCount, "An exception occured during entity loading. See logs for details.");
            //Spawn everything to test spawning
            foreach (string name in EntityLoader.LoadedDefinitionNames)
            {
                EntityCreator.CreateObject(testWorld, name);
                Logger.WriteLine($"Successfully created an instant of {name}.", LogType.LOG);
                Assert.AreEqual(0, ConsoleLogger.ExceptionCount, $"Failed to spawn object with ID '{name}'.");
            }
        }

    }
}
