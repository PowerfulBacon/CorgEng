using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Config
{
    public interface INetworkConfig
    {

        int TickRate { set; get; }

        int PacketMaxSizeBytes { set; get; }

    }
}
