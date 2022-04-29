using CorgEng.GenericInterfaces.UserInterface.Anchors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Components
{
    public interface IUserInterfaceComponent
    {

        /// <summary>
        /// The anchor data for this component
        /// </summary>
        IAnchor Anchor { get; }

        /// <summary>
        /// Render the user interface component
        /// </summary>
        void Render();

        /// <summary>
        /// 
        /// </summary>
        List<IUserInterfaceComponent> Children { get; }

    }
}
