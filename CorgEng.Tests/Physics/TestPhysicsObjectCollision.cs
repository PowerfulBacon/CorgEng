using CorgEng.Physics.Depreciated.Managers;
using CorgEng.Physics.Depreciated.PhysicsObjects;
using CorgEng.UtilityTypes.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.Physics
{
    [TestClass]
    public class TestPhysicsObjectCollision
    {

        [DataTestMethod]
        [DataRow(true, 0, 0, 10, 0, 5, 0, DisplayName = "Test horizontal movement")]
        [DataRow(true, 0, 0, -10, 0, -5, 0, DisplayName = "Test negative horizontal movement")]
        [DataRow(false, 0, 0, 5, 0, 10, 0, DisplayName = "Test horizontal no collision")]
        [DataRow(false, 0, 0, 0, 5, 0, 10, DisplayName = "Test vertical no collision")]
        [DataRow(false, 0, 0, 0, 5, 5, 5, DisplayName = "Test vertical with obstacle in corner")]
        [DataRow(true, 0, 0, 10, 10, 5, 5, DisplayName = "Test diagonal NE movement")]
        [DataRow(true, 0, 0, -10, -10, -5, -5, DisplayName = "Test diagonal SW movement")]
        [DataRow(false, 0, 0, 10, 10, 5, 0, DisplayName = "Test diagonal NE miss")]
        [DataRow(false, 0, 0, 10, 10, 0, 5, DisplayName = "Test diagonal NE miss 2")]
        [DataRow(false, 0, 0, 0, 0, 0, 5, DisplayName = "Test no movement 1")]
        [DataRow(false, 0, 0, 0, 0, 5, 0, DisplayName = "Test no movement 2")]
        [DataRow(false, 0, 0, 0, 0, 5, 5, DisplayName = "Test no movement 3")]
        [Ignore]
        [Obsolete]
        public void TestRectObjectCollision(bool expected, int x1, int y1, int x2, int y2, int ox, int oy)
        {
            PhysicsManager.GetLevel(0).Reset();
            PhysicsObject p1 = new PhysicsObject(new Vector<float>(x1, y1), Hitbox.GetRectHitbox(1, 1, 1, 1));
            new PhysicsObject(new Vector<float>(ox, oy), Hitbox.GetRectHitbox(1, 1, 1, 1));
            Assert.AreEqual(expected, p1.DidIntersectWhenMoving(new Vector<float>(x2, y2)));
        }

    }
}
