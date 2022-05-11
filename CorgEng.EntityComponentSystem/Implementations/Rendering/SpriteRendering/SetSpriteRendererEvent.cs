using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering
{
    public class SetSpriteRendererEvent : Event
    {

        public ISpriteRenderer Target { get; set; }

        public override bool NetworkedEvent => false;

        public SetSpriteRendererEvent(ISpriteRenderer target)
        {
            Target = target;
        }

    }
}
