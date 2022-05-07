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

        /// <summary>
        /// The width of the font texture file
        /// </summary>
        int FontWidth { get; }

        /// <summary>
        /// The height of the font texture file
        /// </summary>
        int FontHeight { get; }

        /// <summary>
        /// The font base
        /// </summary>
        int Base { get; }

    }
}
