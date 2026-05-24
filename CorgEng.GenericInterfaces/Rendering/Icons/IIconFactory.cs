using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Icons
{
    public interface IIconFactory
    {

        IIcon CreateIcon(string name, float layer, int plane);

    }
}
