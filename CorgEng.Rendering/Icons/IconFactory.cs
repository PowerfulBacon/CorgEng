using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Icons;
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

        public IIcon CreateIcon(string name, float layer)
        {
            return new Icon(name, layer);
        }

    }
}
