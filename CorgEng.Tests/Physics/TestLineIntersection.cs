using CorgEng.Physics.Depreciated.PhysicsObjects;
using CorgEng.UtilityTypes.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.Physics
{
    [TestClass]
    public class TestLineIntersection
    {
        [DataTestMethod]
        [DataRow(true, new object[] {
            0, 0, 0, 5,
            10, 0, 10, 5,
            5, -5, 5, 10,
            }, DisplayName = "Test full intersection")]
        [DataRow(true, new object[] {
            10, 0, 10, 5,
            0, 0, 0, 5,
            5, -5, 5, 10,
            }, DisplayName = "Test full intersection backwards")]
        [DataRow(true, new object[] {
            0, 0, 0, 5,
            10, 0, 10, 5,
            5, 2, 5, 3,
            }, DisplayName = "Test internal intersection")]
        [DataRow(false, new object[] {
            0, 0, 0, 5,
            10, 0, 10, 5,
            5, 6, 5, 8,
            }, DisplayName = "Test no intersection 1")]
        [DataRow(false, new object[] {
            0, 0, 0, 5,
            10, 0, 10, 5,
            5, -3, 5, -1,
            }, DisplayName = "Test no intersection 2")]
        [DataRow(true, new object[] {
            10, 0, 10, 5,
            0, 0, 0, 5,
            5, 2, 5, 3,
            }, DisplayName = "Test internal intersection backwards")]
        [DataRow(false, new object[] {
            10, 0, 10, 5,
            0, 0, 0, 5,
            5, 6, 5, 8,
            }, DisplayName = "Test no intersection 1 backwards")]
        [DataRow(false, new object[] {
            10, 0, 10, 5,
            0, 0, 0, 5,
            5, -3, 5, -1,
            }, DisplayName = "Test no intersection 2 backwards")]
        [DataRow(true, new object[] {
            0, 0, 0, 5,
            10, 0, 10, 5,
            5, 3, 5, 8,
            }, DisplayName = "Test partial intersection 1")]
        [DataRow(true, new object[] {
            0, 0, 0, 5,
            10, 0, 10, 5,
            5, -10, 5, 1,
            }, DisplayName = "Test partial intersection 2")]
        [DataRow(true, new object[] {
            0, 0, 0, 5,
            10, 0, 10, 5,
            4, 3, 6, 3,
            }, DisplayName = "Test parallel intersection")]
        [DataRow(true, new object[] {
            0, 0, 0, 5,
            10, 0, 10, 5,
            -5, 3, 15, 3,
            }, DisplayName = "Test parallel long intersection")]
        [DataRow(true, new object[] {
            0, 0, 0, 5,
            10, 0, 10, 5,
            5, 3, 15, 3,
            }, DisplayName = "Test parallel partial long intersection")]
        [DataRow(true, new object[] {
            0, 0, 0, 10,
            10, 0, 10, 10,
            4, 4, 6, 6,
            }, DisplayName = "Test angled intersection")]
        [DataRow(true, new object[] {
            0, 0, 0, 10,
            10, 0, 10, 10,
            -10, -10, 20, 20,
            }, DisplayName = "Test angled intersection starting from outside")]
        [DataRow(true, new object[] {
            0, 0, 0, 10,
            10, 0, 10, 10,
            5, 5, 5, 5,
            }, DisplayName = "Test point inside bounds")]
        [DataRow(false, new object[] {
            0, 0, 0, 10,
            10, 0, 10, 10,
            5, 25, 5, 25,
            }, DisplayName = "Test point outside bounds")]
        [DataRow(false, new object[] {
            0, 0, 0, 10,
            10, 0, 10, 10,
            -10, -20, 20, -10,
            }, DisplayName = "Test angled intersection starting from outside that doesn't intersect")]
        [DataRow(false, new object[] {
            0, 0, 0, 10,
            10, 0, 10, 10,
            3, -10, 7, -1,
            }, DisplayName = "Test angled intersection starting from outside that would intersect if it extended, but doesn't")]
        [DataRow(true, new object[] {
            0, 0, 10, 10,
            0, 10, 10, 20,
            5, 0, 5, 10,
            }, DisplayName = "Test 2 intersected diagonal lines 1")]
        [DataRow(true, new object[] {
            0, 0, 10, 10,
            0, 10, 10, 20,
            0, 5, 10, 5,
            }, DisplayName = "Test 2 intersected diagonal lines 2")]
        [DataRow(true, new object[] {
            0, 0, 10, 10,
            0, 10, 10, 20,
            3, 4, 7, 8,
            }, DisplayName = "Test 2 intersected diagonal lines 3")]
        [DataRow(true, new object[] {
            0, 0, 10, 10,
            0, 10, 10, 20,
            7, 10, 4, 12,
            }, DisplayName = "Test 2 intersected diagonal lines 4")]
        [DataRow(true, new object[] {
            0, 0, 0, 10,
            10, 0, 10, 10,
            5, 3, 5, 7,
            }, DisplayName = "Test parallel vertical lines")]
        [DataRow(true, new object[] {
            0, 0, 10, 0,
            0, 10, 10, 10,
            3, 5, 7, 5,
            }, DisplayName = "Test parallel horizontal lines")]
        [DataRow(true, new object[] {
            0, 0, 0, 10,
            10, 0, 10, 10,
            2, 6, 9, 5,
            }, DisplayName = "Test potentially weird setup 1")]
        [DataRow(true, new object[] {
            0, 0, 0, 10,
            10, 0, 10, 10,
            1, 4, 5, 1,
            }, DisplayName = "Test potentially weird setup 2")]
        [DataRow(false, new object[] {
            1, -1, -1, -1,
            4, -1, 2, -1,
            6, 1, 6, -1,
            }, DisplayName = "Test located issue case 1")]
        [DataRow(false, new object[] {
            1, -1, -1, -1,
            4, -1, 2, -1,
            6, -1, 6, 1,
            }, DisplayName = "Test located issue case 1 (inverted)")]
        [DataRow(false, new object[] {
            -1, -1, 1, -1,
            4, -1, 2, -1,
            6, 1, 6, -1,
            }, DisplayName = "Test located issue case 1.2")]
        [DataRow(false, new object[] {
            -1, -1, 1, -1,
            4, -1, 2, -1,
            6, -1, 6, 1,
            }, DisplayName = "Test located issue case 1.3")]
        [DataRow(false, new object[] {
            -1, -1, 1, -1,
            2, -1, 4, -1,
            6, 1, 6, -1,
            }, DisplayName = "Test located issue case 1.4")]
        [DataRow(false, new object[] {
            -1, -1, 1, -1,
            2, -1, 4, -1,
            6, -1, 6, 1,
            }, DisplayName = "Test located issue case 1.5")]
        [Ignore]
        [Obsolete]
        public void TestLines(bool expected, float a1, float a2, float b1, float b2, float c1, float c2, float d1, float d2, float e1, float e2, float f1, float f2)
        {
            Line startPoint = new Line(new Vector<float>(a1, a2), new Vector<float>(b1, b2));
            Line endPoint = new Line(new Vector<float>(c1, c2), new Vector<float>(d1, d2));
            Line intersector = new Line(new Vector<float>(e1, e2), new Vector<float>(f1, f2));
            Assert.AreEqual(expected, Line.DidLineIntersect(startPoint, endPoint, intersector));
        }

    }
}
