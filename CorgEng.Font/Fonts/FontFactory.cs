using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Font.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Font.Fonts
{
    [Dependency]
    internal class FontFactory : IFontFactory
    {

        public IFont GetFont(string typefaceName)
        {
            return new Font(typefaceName);
        }

    }
}
