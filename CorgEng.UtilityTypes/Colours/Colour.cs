using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.Colours
{
    public class Colour : IColour
    {

        public float Red { get; set; } = 0;

        public float Green { get; set; } = 0;

        public float Blue { get; set; } = 0;

        public Colour(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public Colour()
        {
        }

        public static Colour Parse(string text)
        {
            string textJustNumbers = text.Replace("#", "");
            Colour c = new Colour();
            c.Red = int.Parse(textJustNumbers.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
            c.Green = int.Parse(textJustNumbers.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
            c.Blue = int.Parse(textJustNumbers.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
            return c;
        }

    }
}
