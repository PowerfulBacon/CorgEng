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

        public IUserInterfaceComponent CreateGenericUserInterfaceComponent(IUserInterfaceComponent parent, IAnchor anchorDetails, Action<IUserInterfaceComponent> preInitialiseAction)
        {
            UserInterfaceComponent createdComponent = new UserInterfaceComponent(parent, anchorDetails, Empty);
            preInitialiseAction?.Invoke(createdComponent);
            createdComponent.Initialize();
            return createdComponent;
        }

        public IUserInterfaceComponent CreateGenericUserInterfaceComponent(IAnchor anchorDetails, Action<IUserInterfaceComponent> preInitialiseAction)
        {
            UserInterfaceComponent createdComponent = new UserInterfaceComponent(anchorDetails, Empty);
            preInitialiseAction?.Invoke(createdComponent);
            createdComponent.Initialize();
            return createdComponent;
        }

        public IUserInterfaceComponent CreateUserInterfaceComponent(string componentType, IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments, Action<IUserInterfaceComponent> preInitialiseAction)
        {
            UserInterfaceComponent createdComponent;
            switch (componentType)
            {
                case "BoxComponent":
                    createdComponent = new UserInterfaceBox(parent, anchorDetails, arguments);
                    break;
                case "UserInterface":
                case "UserInterfaceComponent":
                    createdComponent = new UserInterfaceComponent(parent, anchorDetails, arguments);
                    break;
                case "UserInterfaceButton":
                    createdComponent = new UserInterfaceButton(parent, anchorDetails, arguments);
                    break;
                case "DropdownComponent":
                    createdComponent = new UserInterfaceDropdown(parent, anchorDetails, arguments);
                    break;
                case "TextComponent":
                    createdComponent = new UserInterfaceText(parent, anchorDetails, arguments);
                    break;
                case "IconComponent":
                    createdComponent = new UserInterfaceIcon(parent, anchorDetails, arguments);
                    break;
                default:
                    throw new NotImplementedException($"The component {componentType} is not recognised.");
            }
            preInitialiseAction?.Invoke(createdComponent);
            createdComponent.Initialize();
            return createdComponent;
        }

        public IUserInterfaceComponent CreateUserInterfaceComponent(string componentType, IAnchor anchorDetails, IDictionary<string, string> arguments, Action<IUserInterfaceComponent> preInitialiseAction)
        {
            return CreateUserInterfaceComponent(componentType, null, anchorDetails, arguments, preInitialiseAction);
        }
    }
}
