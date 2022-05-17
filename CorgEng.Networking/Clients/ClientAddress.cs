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

        private IntPtr intPtr;

        public unsafe byte* AddressPointer { get; }

        public int AddressBytes { get; }

        public unsafe ClientAddress(int clientIndex)
        {
            int bytesRequired = (int)Math.Ceiling(clientIndex / 8.0);
            AddressBytes = bytesRequired;
            intPtr = Marshal.AllocHGlobal(bytesRequired);
            AddressPointer = (byte*)Marshal.AllocHGlobal(bytesRequired).ToPointer();
        }

        ~ClientAddress()
        {
            //Free the memory
            Marshal.FreeHGlobal(intPtr);
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
                hashCode = unchecked(hashCode * 17 + AddressPointer[i]);
            }
            return hashCode;
        }

    }
}
