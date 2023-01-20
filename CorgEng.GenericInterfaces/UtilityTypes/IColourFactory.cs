using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UtilityTypes
{
    public interface IColourFactory
    {

        IColour GetColour(float r, float g, float b, float a = 1);

    }
}
