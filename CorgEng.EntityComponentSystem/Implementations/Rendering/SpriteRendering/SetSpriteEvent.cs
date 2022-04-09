using CorgEng.EntityComponentSystem.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering
{
    public class SetSpriteEvent : Event
    {

        public string TextureFile { get; set; }

        public SetSpriteEvent(string textureFile)
        {
            TextureFile = textureFile;
        }
    }
}
