using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Components
{
    [Dependency]
    internal class UserInterfaceComponentFactory : IUserInterfaceComponentFactory
    {

        public IUserInterfaceComponent CreateGenericUserInterfaceComponent(IUserInterfaceComponent parent, IAnchor anchorDetails)
        {
            return new UserInterfaceComponent(parent, anchorDetails);
        }

        public IUserInterfaceComponent CreateGenericUserInterfaceComponent(IAnchor anchorDetails)
        {
            return new UserInterfaceComponent(anchorDetails);
        }

    }
}
