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

        public IUserInterfaceComponent CreateUserInterfaceComponent(string componentType, IUserInterfaceComponent parent, IAnchor anchorDetails, params KeyValuePair<string, string>[] arguments)
        {
            switch (componentType)
            {
                case "BoxComponent":
                    return new UserInterfaceBox(parent, anchorDetails);
                case "UserInterface":
                case "UserInterfaceComponent":
                    return new UserInterfaceComponent(parent, anchorDetails);
                default:
                    throw new NotImplementedException($"The component {componentType} is not recognised.");
            }
        }

        public IUserInterfaceComponent CreateUserInterfaceComponent(string componentType, IAnchor anchorDetails, params KeyValuePair<string, string>[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}
