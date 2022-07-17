using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class DeleteEntityEvent : INetworkedEvent
    {
        public bool CanBeRaisedByClient => false;

        public void Deserialise(BinaryReader reader)
        {
            return;
        }

        public void Serialise(BinaryWriter writer)
        {
            return;
        }

        public int SerialisedLength()
        {
            return 0;
        }

    }
}
