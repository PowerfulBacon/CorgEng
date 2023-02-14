using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Colours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Lighting.Components
{
    public class LightingComponent : Component
    {

        public float Radius { get; set; } = 8;

        public IColour Colour { get; set; } = new Colour(252 / 255f, 220 / 255f, 164 / 255f, 1.0f);

    }
}
