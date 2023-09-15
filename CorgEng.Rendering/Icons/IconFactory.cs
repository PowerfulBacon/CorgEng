using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Icons
{
    [Dependency]
    internal class IconFactory : IIconFactory
    {

        public IIcon CreateIcon(string name, float layer, int plane)
        {
            return new Icon(name, layer, plane);
        }

    }
}
