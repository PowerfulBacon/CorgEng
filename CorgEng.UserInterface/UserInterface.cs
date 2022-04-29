using CorgEng.GenericInterfaces.UserInterface;
using CorgEng.GenericInterfaces.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface
{
    internal class UserInterface : IUserInterface
    {

        public IUserInterfaceComponent Root { get; set; } = null;

        public void Render()
        {
            if (Root == null)
                return;
            Root.Render();
        }

    }
}
