using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Colours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Style
{
    internal class BoxStyle : Style
    {

        public float BorderWidth { get; set; } = 5;

        public IColour BorderColour { get; set; } = new Colour(1, 1, 1);

        public IColour FillColour { get; set; } = new Colour(0, 0, 0);

        /// <summary>
        /// Colour change when hovered
        /// </summary>
        public IColour HoverColour { get; set; } = null;

        public override void ParseSettings(IDictionary<string, string> settings)
        {
            string output;
            if (settings.TryGetValue("borderWidth", out output))
                BorderWidth = float.Parse(output);
            if (settings.TryGetValue("borderColour", out output))
                BorderColour = Colour.Parse(output);
            if (settings.TryGetValue("fillColour", out output))
                FillColour = Colour.Parse(output);
            if (settings.TryGetValue("hoverColour", out output))
                HoverColour = Colour.Parse(output);
        }

    }
}
