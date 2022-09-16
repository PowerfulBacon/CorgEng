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
        private static IClientAddressingTableFactory ClientAddressingTableFactory;

        private IClientAddressingTable ClientAddressingTable;

        [UsingDependency]
        private static IClientAddressFactory ClientAddressFactory;

        [UsingDependency]
        private static IClientFactory ClientFactory;

        [TestInitialize]
        public void AfterTests()
        {
            ClientAddressingTable = ClientAddressingTableFactory.CreateAddressingTable();
            //Fully reset the client addressing table
            ClientAddressingTable.Reset();
        }

        [TestMethod]
        [Timeout(10000)]
        public void TestHashing()
        {
            IClient dummyClient = GetDummyClient();
            IClientAddress createdAddressA = ClientAddressFactory.CreateAddress(3, dummyClient);
            IClientAddress createdAddressB = ClientAddressFactory.CreateAddress(3, dummyClient);
            IClientAddress createdAddressC = ClientAddressFactory.CreateAddress(4, dummyClient);
            Assert.AreEqual(createdAddressA.GetHashCode(), createdAddressB.GetHashCode());
            Assert.AreNotEqual(createdAddressB.GetHashCode(), createdAddressC.GetHashCode());

            IClientAddress referenceAddress = ClientAddressFactory.CreateAddress(0, dummyClient);
            for (int i = 0; i < 100; i++)
            {
                IClientAddress addressA = ClientAddressFactory.CreateAddress(i, dummyClient);
                IClientAddress addressB = ClientAddressFactory.CreateAddress(i, dummyClient);
                Assert.AreEqual(addressA.GetHashCode(), addressB.GetHashCode());
                Assert.IsTrue(addressA.Equals(addressB));
                Assert.IsTrue(addressB.Equals(addressA));
                Assert.AreEqual(referenceAddress.GetHashCode(), ClientAddressFactory.CreateAddress(0, dummyClient).GetHashCode());
                Assert.AreNotEqual(referenceAddress.GetHashCode(), ClientAddressFactory.CreateAddress(i + 1, dummyClient).GetHashCode());
            }
        }

        [TestMethod]
        public void TestDictionary()
        {
            IClient dummyClient = GetDummyClient();
            Dictionary<IClientAddress, bool> dictionary = new Dictionary<IClientAddress, bool>();
            dictionary.Add(ClientAddressFactory.CreateAddress(3, dummyClient), true);
            dictionary.Add(ClientAddressFactory.CreateAddress(4, dummyClient), false);
            int sanity = 3;
            while (dictionary.Count > 0 && sanity-- > 0)
            {
                KeyValuePair<IClientAddress, bool> firstElement = dictionary.First();
                dictionary.Remove(firstElement.Key);
            }
            if (sanity <= 0)
                Assert.Fail("Sanity failed, could not remove from dictionary");
            //Test clashing addresses
            dictionary.Add(ClientAddressFactory.CreateAddress(3, dummyClient), true);
            Assert.ThrowsException<ArgumentException>(() => {
                dictionary.Add(ClientAddressFactory.CreateAddress(3, dummyClient), false);
            });
        }

        [TestMethod]
        [Timeout(10000)]
        public void TestEquality()
        {
            IClient dummyClient = GetDummyClient();
            Assert.AreEqual(ClientAddressFactory.CreateAddress(3, dummyClient), ClientAddressFactory.CreateAddress(3, dummyClient));
            dummyClient = GetDummyClient();
            Assert.AreEqual(ClientAddressFactory.CreateAddress(7, dummyClient), ClientAddressFactory.CreateAddress(7, dummyClient));
            dummyClient = GetDummyClient();
            Assert.AreEqual(ClientAddressFactory.CreateAddress(8, dummyClient), ClientAddressFactory.CreateAddress(8, dummyClient));
            dummyClient = GetDummyClient();
            Assert.AreEqual(ClientAddressFactory.CreateAddress(11, dummyClient), ClientAddressFactory.CreateAddress(11, dummyClient));
            dummyClient = GetDummyClient();
            Assert.AreEqual(ClientAddressFactory.CreateAddress(110, dummyClient), ClientAddressFactory.CreateAddress(110, dummyClient));
        }

        [TestMethod]
        [Timeout(10000)]
        public unsafe void TestEnablingFlag()
        {
            IClientAddress flagTest = ClientAddressFactory.CreateAddress(0, GetDummyClient());
            IClientAddress clientAddress1 = ClientAddressFactory.CreateAddress(1, GetDummyClient());
            IClientAddress clientAddress3 = ClientAddressFactory.CreateAddress(3, GetDummyClient());
            //Enable the flag
            flagTest.EnableFlag(clientAddress1);
            flagTest.EnableFlag(clientAddress3);
            Assert.AreEqual(0b00000101, flagTest.AddressPointer[0]);
        }

        [TestMethod]
        [Timeout(10000)]
        public unsafe void TestDisablingFlag()
        {
            IClientAddress flagTest = ClientAddressFactory.CreateAddress(0, GetDummyClient());
            IClientAddress clientAddress1 = ClientAddressFactory.CreateAddress(1, GetDummyClient());
            IClientAddress clientAddress3 = ClientAddressFactory.CreateAddress(3, GetDummyClient());
            //Enable the flag
            flagTest.EnableFlag(clientAddress1);
            flagTest.EnableFlag(clientAddress3);
            flagTest.DisableFlag(ClientAddressFactory.CreateAddress(1, GetDummyClient()));
            Assert.AreEqual(0b00000100, flagTest.AddressPointer[0]);
        }

        [TestMethod]
        [Timeout(10000)]
        public unsafe void TestAddressGetting()
        {
            Assert.IsNotNull(ClientAddressingTable, "Dependency injection failed");
            //Assign some addresses
            IClientAddress clientAddress;
            clientAddress = ClientAddressingTable.AddClient(GetDummyClient());
            //Check the expected value
            Assert.AreEqual(0b00000001, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddClient(GetDummyClient());
            //Check the expected value
            Assert.AreEqual(0b00000010, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddClient(GetDummyClient());
            //Check the expected value
            Assert.AreEqual(0b00000100, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddClient(GetDummyClient());
            //Check the expected value
            Assert.AreEqual(0b00001000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddClient(GetDummyClient());
            //Check the expected value
            Assert.AreEqual(0b00010000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddClient(GetDummyClient());
            //Check the expected value
            Assert.AreEqual(0b00100000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddClient(GetDummyClient());
            //Check the expected value
            Assert.AreEqual(0b01000000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddClient(GetDummyClient());
            //Check the expected value
            Assert.AreEqual(0b10000000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[1]);
            clientAddress = ClientAddressingTable.AddClient(GetDummyClient());
            //Check the expected value
            Assert.AreEqual(0b00000000, clientAddress.AddressPointer[0]);
            Assert.AreEqual(0b00000001, clientAddress.AddressPointer[1]);
        }

        [TestMethod]
        [Timeout(10000)]
        public unsafe void TestEveryone()
        {
            Assert.IsNotNull(ClientAddressingTable, "Dependency injection failed");
            //Assign some addresses
            ClientAddressingTable.AddClient(GetDummyClient());
            ClientAddressingTable.AddClient(GetDummyClient());
            ClientAddressingTable.AddClient(GetDummyClient());
            ClientAddressingTable.AddClient(GetDummyClient());
            ClientAddressingTable.AddClient(GetDummyClient());
            ClientAddressingTable.AddClient(GetDummyClient());
            ClientAddressingTable.AddClient(GetDummyClient());
            ClientAddressingTable.AddClient(GetDummyClient());
            ClientAddressingTable.AddClient(GetDummyClient());
            //Check the everyone address
            Assert.AreEqual(0b11111111, ClientAddressingTable.GetEveryone().AddressPointer[0]);
            Assert.AreEqual(0b00000001, ClientAddressingTable.GetEveryone().AddressPointer[1]);
        }

        private IClient GetDummyClient()
        {
            return ClientFactory.CreateClient("test", new IPEndPoint(IPAddress.Parse("0.0.0.1"), 5000));
        }

    }
}
