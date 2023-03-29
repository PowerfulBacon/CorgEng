using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Components.ComponentVariables
{
    internal interface IComponentVariable
    {

        /// <summary>
        /// Associate this component variable with a specific component.
        /// Called via reflection.
        /// </summary>
        /// <param name="component"></param>
        void AssociateTo(Component component);

    }
}
