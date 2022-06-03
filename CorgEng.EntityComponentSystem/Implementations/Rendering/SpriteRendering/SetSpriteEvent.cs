using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering
{
    public class SetSpriteEvent : INetworkedEvent
    {

        public string TextureFile { get; set; }

        public SetSpriteEvent(string textureFile)
        {
            TextureFile = textureFile;
        }

        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(TextureFile);
        }

        public void Deserialize(byte[] data)
        {
            TextureFile = Encoding.UTF8.GetString(data);
        }
    }
}
