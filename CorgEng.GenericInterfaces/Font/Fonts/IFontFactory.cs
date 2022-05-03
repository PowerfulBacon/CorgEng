using CorgEng.GenericInterfaces.Font.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Font.Fonts
{
    public interface IFontFactory
    {

        IFont GetFont(string typefaceName);

    }
}
