using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    public class SetRotationEvent : IEvent
    {

        public Vector<float> NewRotation { get; set; }

        public SetRotationEvent(Vector<float> newRotation)
        {
            NewRotation = newRotation;
        }

    }
}
