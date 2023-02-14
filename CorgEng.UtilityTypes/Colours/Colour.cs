using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
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

        public float Alpha { get; set; } = 1;

        public Colour(float red, float green, float blue, float alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

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
            if (textJustNumbers.Length == 8)
            {
                c.Alpha = int.Parse(textJustNumbers.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
            }
            return c;
        }

        public override bool Equals(object obj)
        {
            return obj is Colour colour &&
                   Red == colour.Red &&
                   Green == colour.Green &&
                   Blue == colour.Blue &&
                   Alpha == colour.Alpha;
        }

        public IVector<float> ToVector()
        {
            return new Vector<float>(
                Red,
                Green,
                Blue,
                Alpha
                );
        }
    }
}
