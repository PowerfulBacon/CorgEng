﻿using CorgEng.GenericInterfaces.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Networking
{

    public delegate void NetworkMessageRecieved(PacketHeaders packetHeader, byte[] message, int dataStart, int dataLength);

}
