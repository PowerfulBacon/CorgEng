using CorgEng.EntityComponentSystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Components
{
    internal class UserInterfaceClickActionComponent : Component
    {

        public Action InvokationAction { get; }

        public UserInterfaceClickActionComponent(Action invokationAction)
        {
            InvokationAction = invokationAction;
        }
    }
}
