using CorgEng.GenericInterfaces.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface
{
    public interface IUserInterface
    {

        /// <summary>
        /// The root component for the user interface
        /// </summary>
        IUserInterfaceComponent Root { get; }

        /// <summary>
        /// Render this user interface
        /// </summary>
        void Render();

    }
}
