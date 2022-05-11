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

        public override bool NetworkedEvent => true;

        public SetSpriteEvent(string textureFile)
        {
            TextureFile = textureFile;
        }

        public override byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(TextureFile);
        }

        public override void Deserialize(byte[] data)
        {
            TextureFile = Encoding.UTF8.GetString(data);
        }
    }
}
