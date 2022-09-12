using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Components
{
    /// <summary>
    /// For user interface components that are clickable.
    /// </summary>
    internal class UserInterfaceClickerComponent : Component
    {

        public MethodInfo InvokationMethod { get; }

        public IUserInterfaceComponent ClickedComponent { get; }

        public UserInterfaceClickerComponent(MethodInfo invokationMethod, IUserInterfaceComponent clickedComponent)
        {
            InvokationMethod = invokationMethod;
            ClickedComponent = clickedComponent;
        }
    }
}
