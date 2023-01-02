using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.World.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.ClickHandler
{
    internal class SelectableComponent : TrackComponent
    {

        public const string TRACK_KEY = "_selectable";

        public SelectableComponent() : base(TRACK_KEY)
        {
        }

    }
}
