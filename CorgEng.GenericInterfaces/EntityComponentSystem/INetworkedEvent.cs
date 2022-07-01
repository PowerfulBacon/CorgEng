using CorgEng.GenericInterfaces.Networking.VersionSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface INetworkedEvent : IEvent, IVersionSynced
    {

        bool CanBeRaisedByClient { get; }

        void Deserialize(byte[] data);

        byte[] Serialize();

    }
}
