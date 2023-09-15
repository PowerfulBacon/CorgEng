using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Generators
{
    public interface IUserInterfaceXmlLoader
    {

        /// <summary>
        /// Load a user interface from a provided XML file.
        /// </summary>
        /// <param name="filepath">The path to the XML file to load the user interface from.</param>
        /// <returns>Returns the root component of the user interface.</returns>
        IUserInterfaceComponent LoadUserInterface(IWorld world, string filepath);

    }
}
