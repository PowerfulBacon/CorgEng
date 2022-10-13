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

        private static Dictionary<string, Font> FontCache = new Dictionary<string, Font>();

        public IFont GetFont(string typefaceName)
        {
            if (FontCache.ContainsKey(typefaceName))
            {
                return FontCache[typefaceName];
            }
            lock (FontCache)
            {
                if (FontCache.ContainsKey(typefaceName))
                {
                    return FontCache[typefaceName];
                }
                Font createdFont = new Font(typefaceName);
                FontCache.Add(typefaceName, createdFont);
                return createdFont;
            }
        }

    }
}
