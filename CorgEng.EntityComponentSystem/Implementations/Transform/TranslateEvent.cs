using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    public sealed class TranslateEvent : IEvent
    {

        public Vector<float> TranslationDelta { get; set; }

        public TranslateEvent(Vector<float> translationDelta)
        {
            TranslationDelta = translationDelta;
        }

    }
}
