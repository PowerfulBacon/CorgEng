using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class DeleteEntityEvent : Event
    {

        public override bool IsSynced { get; } = true;

        public override byte[] Serialize()
        {
            return new byte[0];
        }

        public override void Deserialize(byte[] data)
        { }
    }
}
