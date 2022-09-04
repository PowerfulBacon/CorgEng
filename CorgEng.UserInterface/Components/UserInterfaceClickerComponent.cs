using CorgEng.EntityComponentSystem.Components;
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

        public UserInterfaceClickerComponent(MethodInfo invokationMethod)
        {
            InvokationMethod = invokationMethod;
        }
    }
}
