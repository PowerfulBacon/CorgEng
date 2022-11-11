using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.UserInterface.UserInterfaceComponents;
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

        private static Dictionary<string, string> Empty { get; } = new Dictionary<string, string>();

        public IUserInterfaceComponent CreateGenericUserInterfaceComponent(IUserInterfaceComponent parent, IAnchor anchorDetails)
        {
            return new UserInterfaceComponent(parent, anchorDetails, Empty);
        }

        public IUserInterfaceComponent CreateGenericUserInterfaceComponent(IAnchor anchorDetails)
        {
            return new UserInterfaceComponent(anchorDetails, Empty);
        }

        public IUserInterfaceComponent CreateUserInterfaceComponent(string componentType, IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments)
        {
            switch (componentType)
            {
                case "BoxComponent":
                    return new UserInterfaceBox(parent, anchorDetails, arguments);
                case "UserInterface":
                case "UserInterfaceComponent":
                    return new UserInterfaceComponent(parent, anchorDetails, arguments);
                case "UserInterfaceButton":
                    return new UserInterfaceButton(parent, anchorDetails, arguments);
                case "DropdownComponent":
                    return new UserInterfaceDropdown(parent, anchorDetails, arguments);
                case "TextComponent":
                    return new UserInterfaceText(parent, anchorDetails, arguments);
                case "IconComponent":
                    return new UserInterfaceIcon(parent, anchorDetails, arguments);
                default:
                    throw new NotImplementedException($"The component {componentType} is not recognised.");
            }
        }

        public IUserInterfaceComponent CreateUserInterfaceComponent(string componentType, IAnchor anchorDetails, IDictionary<string, string> arguments)
        {
            throw new NotImplementedException();
        }
    }
}
