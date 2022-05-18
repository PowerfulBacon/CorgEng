using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Networking.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.NetworkingTests
{
    [TestClass]
    public class ClientAddressTests
    {

        [UsingDependency]
        private static IClientAddressingTable ClientAddressingTable;

        [UsingDependency]
        private static IClientAddressFactory ClientAddressFactory;

        [TestInitialize]
        public void AfterTests()
        {
            //Fully reset the client addressing table
            ClientAddressingTable.Reset();
        }

        [TestMethod]
        public void TestEquality()
        {
            Assert.AreEqual(ClientAddressFactory.CreateAddress(3), ClientAddressFactory.CreateAddress(3));
            Assert.AreEqual(ClientAddressFactory.CreateAddress(7), ClientAddressFactory.CreateAddress(7));
            Assert.AreEqual(ClientAddressFactory.CreateAddress(8), ClientAddressFactory.CreateAddress(8));
            Assert.AreEqual(ClientAddressFactory.CreateAddress(11), ClientAddressFactory.CreateAddress(11));
            Assert.AreEqual(ClientAddressFactory.CreateAddress(110), ClientAddressFactory.CreateAddress(110));
        }

        [TestMethod]
        public unsafe void TestEnablingFlag()
        {
            IClientAddress flagTest = ClientAddressFactory.CreateAddress(0);
            IClientAddress clientAddress1 = ClientAddressFactory.CreateAddress(1);
            IClientAddress clientAddress3 = ClientAddressFactory.CreateAddress(3);
            //Enable the flag
            flagTest.EnableFlag(clientAddress1);
            flagTest.EnableFlag(clientAddress3);
            Assert.AreEqual(0b00000101, flagTest.AddressPointer[0]);
        }

        [TestMethod]
        public unsafe void TestDisablingFlag()
        {
            IClientAddress flagTest = ClientAddressFactory.CreateAddress(0);
            IClientAddress clientAddress1 = ClientAddressFactory.CreateAddress(1);
            IClientAddress clientAddress3 = ClientAddressFactory.CreateAddress(3);
            //Enable the flag
            flagTest.EnableFlag(clientAddress1);
            flagTest.EnableFlag(clientAddress3);
            flagTest.DisableFlag(ClientAddressFactory.CreateAddress(1));
            Assert.AreEqual(0b00000100, flagTest.AddressPointer[0]);
        }

        [TestMethod]
        public unsafe void TestAddressGetting()
        {
            Assert.IsNotNull(ClientAddressingTable, "Dependency injection failed");
            //Assign some addresses
            IClientAddress clientAddress;
            clientAddress = ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.1"));
            //Check the expected value
            Assert.AreEqual(0b00000001, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.2"));
            //Check the expected value
            Assert.AreEqual(0b00000010, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.3"));
            //Check the expected value
            Assert.AreEqual(0b00000100, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.4"));
            //Check the expected value
            Assert.AreEqual(0b00001000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.5"));
            //Check the expected value
            Assert.AreEqual(0b00010000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.6"));
            //Check the expected value
            Assert.AreEqual(0b00100000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.7"));
            //Check the expected value
            Assert.AreEqual(0b01000000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.8"));
            //Check the expected value
            Assert.AreEqual(0b10000000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.9"));
            //Check the expected value
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000001, clientAddress.AddressPointer[1]);
        }

        [TestMethod]
        public unsafe void TestEveryone()
        {
            Assert.IsNotNull(ClientAddressingTable, "Dependency injection failed");
            //Assign some addresses
            ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.1"));
            ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.2"));
            ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.3"));
            ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.4"));
            ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.5"));
            ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.6"));
            ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.7"));
            ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.8"));
            ClientAddressingTable.AddAddress(IPAddress.Parse("0.0.0.9"));
            //Check the everyone address
            Assert.AreEqual(0b11111111, ClientAddressingTable.GetEveryone().AddressPointer[0]);
            Assert.AreEqual(0b00000001, ClientAddressingTable.GetEveryone().AddressPointer[1]);
        }

    }
}
