using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Colours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer.Box
{
    internal class UserInterfaceBoxRenderObject : UserInterfaceRenderObject
    {

        public float BorderWidth { get; set; } = 5;

        public IColour BorderColour { get; set; } = new Colour(1, 1, 1);

        public IColour FillColour { get; set; } = new Colour(0, 0, 0);

    }
}
