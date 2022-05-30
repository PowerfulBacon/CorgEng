using CorgEng.EntityComponentSystem.Events;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    public sealed class TranslateEvent : Event
    {

        public Vector<float> TranslationDelta { get; set; }

        public override bool IsSynced => false;

        public TranslateEvent(Vector<float> translationDelta)
        {
            TranslationDelta = translationDelta;
        }

    }
}
