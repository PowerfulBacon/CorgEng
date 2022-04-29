using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Components
{
    internal class UserInterfaceComponent : IUserInterfaceComponent
    {

        /// <summary>
        /// The anchor for this component
        /// </summary>
        public IAnchor Anchor { get; }

        /// <summary>
        /// The render core of the component
        /// </summary>
        public RenderCore ComponentRenderCore { get; }

        public UserInterfaceComponent(RenderCore renderCore)
        {
            //Set the render core
            ComponentRenderCore = renderCore;
        }

        /// <summary>
        /// Perform rendering of this component
        /// </summary>
        public void Render()
        {
            ComponentRenderCore.PerformRender();
        }

    }
}
