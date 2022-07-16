using CorgEng.GenericInterfaces.Networking.VersionSync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface INetworkedEvent : IEvent, IVersionSynced
    {

        bool CanBeRaisedByClient { get; }

        /// <summary>
        /// Deseriliase information from the reader
        /// </summary>
        /// <param name="reader"></param>
        void Deserialise(BinaryReader reader);

        /// <summary>
        /// Serialises this event into the provided bytestream.
        /// </summary>
        /// <param name="writer"></param>
        void Serialise(BinaryWriter writer);

        /// <summary>
        /// The serialised length of the event.
        /// </summary>
        /// <returns></returns>
        int SerialisedLength();

    }
}
