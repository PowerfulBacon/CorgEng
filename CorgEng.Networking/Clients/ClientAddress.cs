using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Clients
{
    internal class ClientAddress : IClientAddress
    {

        //Prevent GC
        private byte[] byteArray;

        public unsafe byte* AddressPointer { get; private set; }

        public int AddressBytes { get; private set; }

        private List<IClient> clients = new List<IClient>();

        public unsafe ClientAddress(int clientIndex, IClient client)
        {
            int bytesRequired = Math.Max((int)Math.Ceiling(clientIndex / 8.0), 1);
            AddressBytes = bytesRequired;

            if (client != null)
                clients.Add(client);

            byteArray = new byte[bytesRequired];
            fixed (byte* bytePointer = byteArray)
            {
                AddressPointer = bytePointer;
            }
            if(clientIndex != 0)
                AddressPointer[bytesRequired - 1] = (byte)Math.Pow(2, ((clientIndex - 1) % 8));
        }

        public unsafe override bool Equals(object obj)
        {
            if (obj is ClientAddress clientAddress)
            {
                //Ensure the bytes are all the same
                for (int i = 0; i < Math.Max(AddressBytes, clientAddress.AddressBytes); i++)
                {
                    byte firstByte = i < AddressBytes ? AddressPointer[i] : (byte)0;
                    byte secondByte = i < clientAddress.AddressBytes ? clientAddress.AddressPointer[i] : (byte)0;
                    if (firstByte != secondByte)
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// uhhh this works right?
        /// </summary>
        public unsafe override int GetHashCode()
        {
            int hashCode = -1466858141;
            for (int i = 0; i < AddressBytes; i++)
            {
                hashCode = unchecked(hashCode * 17 + byteArray[i]);
            }
            return hashCode;
        }

        public unsafe bool HasFlag(IClientAddress searchingFor)
        {
            for (int i = 0; i < searchingFor.AddressBytes; i++)
            {
                byte searchingForByte = searchingFor.AddressPointer[i];
                if (i >= AddressBytes && searchingForByte > 0)
                    return false;
                if ((AddressPointer[i] & searchingForByte) != searchingForByte)
                    return false;
            }
            return true;
        }

        public unsafe void EnableFlag(IClientAddress enablingFlag)
        {
            //Check if we need to allocate more memory for this flag
            if (enablingFlag.AddressBytes > AddressBytes)
            {
                //Old memory will be garbage collected
                //Allocate new memory
                AddressBytes = enablingFlag.AddressBytes;
                //Store temporarilly for copying across
                byte[] temp = byteArray;
                //Create a new byte array
                byteArray = new byte[AddressBytes];
                temp.CopyTo(byteArray, 0);
                fixed (byte* bytePointer = byteArray)
                {
                    AddressPointer = bytePointer;
                }
            }
            //Enable the flags
            for (int i = 0; i < AddressBytes; i++)
            {
                AddressPointer[i] |= enablingFlag.AddressPointer[i];
            }
            //Add all clients
            clients.AddRange(enablingFlag.GetClients());
        }

        public unsafe void DisableFlag(IClientAddress disablingFlag)
        {
            //Locate and disable the specified flag
            for (int i = 0; i < Math.Min(AddressBytes, disablingFlag.AddressBytes); i++)
            {
                AddressPointer[i] &= (byte)~disablingFlag.AddressPointer[i];
            }
            //Remove all clients
            foreach (IClient client in disablingFlag.GetClients())
            {
                clients.Remove(client);
            }
        }

        public IEnumerable<IClient> GetClients()
        {
            return clients;
        }
    }
}
