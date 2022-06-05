using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Prototypes
{
    internal class Prototype : IPrototype
    {

        private static uint PrototypeIdentifierHighest = 0;

        public uint Identifier { get; set; } = PrototypeIdentifierHighest++;

        public IEntity CreateEntityFromPrototype()
        {
        }

        public void DeserializePrototype(byte[] data)
        {
        }

        public byte[] SerializePrototype()
        {
        }

    }
}
