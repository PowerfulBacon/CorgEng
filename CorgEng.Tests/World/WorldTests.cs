﻿using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.World;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.World
{
    [TestClass]
    public class WorldTests
    {

        [UsingDependency]
        public static IWorld WorldAccess;

        [TestMethod]
        public void TestWorld()
        {
            Assert.IsNotNull(WorldAccess, "Dependency injection failed");
            //Create some entities
            IEntity entityA = new Entity();
            IEntity entityB = new Entity();
            IEntity entityC = new Entity();
            IEntity entityD = new Entity();
            IEntity entityE = new Entity();
            IEntity entityF = new Entity();
            IEntity entityG = new Entity();
            //Do some adding and removing
            WorldAccess.AddEntity(entityA, 1, 1, 0);
            WorldAccess.AddEntity(entityB, 1, 1, 0);
            WorldAccess.AddEntity(entityC, -1.3, 0, 0);
            WorldAccess.AddEntity(entityE, -1, 0, 0);
            WorldAccess.AddEntity(entityD, 0, 0, 6);
            WorldAccess.AddEntity("FG", entityF, 0, 0, 6);
            WorldAccess.AddEntity("FG", entityG, 0, 4, 6);
            WorldAccess.RemoveEntity(entityC, -1.3, 0, 0);
            WorldAccess.RemoveEntity("FG", entityF, 0, 0, 6);
            WorldAccess.AddEntity("FG", entityF, 0, 0, 7);
            WorldAccess.AddEntity(entityC, -1.3, 0, 0);
            WorldAccess.RemoveEntity(entityC, -1.3, 0, 0);
            //Test what we expect
            //(1, 1, 0)
            bool foundA = false;
            bool foundB = false;
            foreach (IEntity entity in WorldAccess.GetContentsAt(1, 1, 0).GetContents())
            {
                if (entity == entityA && !foundA)
                    foundA = true;
                else if (entity == entityB && !foundB)
                    foundB = true;
                else
                    Assert.Fail($"Located an entity that was not meant to be at (1, 1, 0). FoundA: {foundA}, FoundB: {foundB}, Entity Found: {entity}");
            }
            //(-1, 0, 0)
            bool foundE = false;
            foreach (IEntity entity in WorldAccess.GetContentsAt(-1, 0, 0).GetContents())
            {
                if (entity == entityE && !foundE)
                    foundE = true;
                else
                    Assert.Fail($"Located an entity that was not meant to be at (-1, 0, 0). foundE: {foundE}, Entity Found: {entity}");
            }
            //(0, 0, 6)
            bool foundD = false;
            foreach (IEntity entity in WorldAccess.GetContentsAt(0, 0, 6).GetContents())
            {
                if (entity == entityD && !foundD)
                    foundD = true;
                else
                    Assert.Fail($"Located an entity that was not meant to be at (0, 0, 6). foundD: {foundD}, Entity Found: {entity}");
            }
            //('FG', 0, 0, 7)
            bool foundF = false;
            foreach (IEntity entity in WorldAccess.GetContentsAt("FG", 0, 0, 7).GetContents())
            {
                if (entity == entityF && !foundF)
                    foundF = true;
                else
                    Assert.Fail($"Located an entity that was not meant to be at ('FG', 0, 0, 7). foundF: {foundF}, Entity Found: {entity}");
            }
            //('FG', 0, 4, 6)
            bool foundG = false;
            foreach (IEntity entity in WorldAccess.GetContentsAt("FG", 0, 4, 6).GetContents())
            {
                if (entity == entityG && !foundG)
                    foundG = true;
                else
                    Assert.Fail($"Located an entity that was not meant to be at ('FG', 0, 4, 7). foundG: {foundG}, Entity Found: {entity}");
            }
            Assert.IsTrue(foundA, $"Could not locate A. There were {WorldAccess.GetContentsAt(1, 1, 0).GetContents().Count()} elements located at (1, 1, 0)");
            Assert.IsTrue(foundB, $"Could not locate B. There were {WorldAccess.GetContentsAt(1, 1, 0).GetContents().Count()} elements located at (1, 1, 0)");
            Assert.IsTrue(foundD, $"Could not locate D. There were {WorldAccess.GetContentsAt(0, 0, 6).GetContents().Count()} elements located at (0, 0, 6)");
            Assert.IsTrue(foundE, $"Could not locate E. There were {WorldAccess.GetContentsAt(-1, 0, 0).GetContents().Count()} elements located at (-1, 0, 0)");
            Assert.IsTrue(foundF, $"Could not locate F. There were {WorldAccess.GetContentsAt("FG", 0, 0, 7).GetContents().Count()} elements located at ('FG', 0, 0, 7)");
            Assert.IsTrue(foundG, $"Could not locate G. There were {WorldAccess.GetContentsAt("FG", 0, 4, 6).GetContents().Count()} elements located at ('FG', 0, 4, 7)");
        }

    }
}
