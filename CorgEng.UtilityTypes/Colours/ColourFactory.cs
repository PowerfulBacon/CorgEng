using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.Colours
{
    [Dependency]
    internal class ColourFactory : IColourFactory
    {
        public IColour GetColour(float r, float g, float b, float a = 1)
        {
            return new Colour(r, g, b, a);
        }
    }
}
