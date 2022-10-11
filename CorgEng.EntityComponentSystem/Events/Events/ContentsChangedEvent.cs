using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    /// <summary>
    /// Called on an entity when it leaves another entities contents.
    /// </summary>
    public class ContentsChangedEvent : INetworkedEvent
    {

        [UsingDependency]
        private static IAutoSerialiser AutoSerialiser = null!;

        public bool CanBeRaisedByClient => false;

        public IEntity OldHolder { get; }

        public IEntity NewHolder { get; }

        public ContentsChangedEvent(IEntity oldHolder, IEntity newHolder)
        {
            OldHolder = oldHolder;
            NewHolder = newHolder;
        }

        public void Deserialise(BinaryReader reader)
        {
            throw new NotImplementedException("This method has not been implemented yet due to not figuring out " +
                "what to do if an entity hasn't been recieved from the server yet, and networking beinga low priority at this time.");
        }

        public void Serialise(BinaryWriter writer)
        {
            AutoSerialiser.SerializeInto(typeof(uint?), OldHolder?.Identifier, writer);
            AutoSerialiser.SerializeInto(typeof(uint?), NewHolder?.Identifier, writer);
        }

        public int SerialisedLength()
        {
            return AutoSerialiser.SerialisationLength(typeof(uint?), OldHolder?.Identifier)
                + AutoSerialiser.SerialisationLength(typeof(uint?), NewHolder?.Identifier);
        }
    }
}
