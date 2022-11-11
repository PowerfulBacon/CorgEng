using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer.Icon
{
    internal class UserInterfaceIconRenderObject : UserInterfaceRenderObject
    {

        public ITextureState Texture { get; set; }

        public UserInterfaceIconRenderObject(ITextureState texture)
        {
            Texture = texture;
        }
    }
}
