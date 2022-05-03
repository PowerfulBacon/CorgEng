using CorgEng.GenericInterfaces.Font.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Font.Fonts
{
    public interface IFont
    {

        /// <summary>
        /// Gets the specified data about a specific character
        /// </summary>
        IFontCharacter GetCharacter(int code);

    }
}
