using CorgEng.GenericInterfaces.UserInterface.Anchors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Components
{
    public interface IUserInterfaceComponentFactory
    {

        IUserInterfaceComponent CreateGenericUserInterfaceComponent(IUserInterfaceComponent parent, IAnchor anchorDetails);

        IUserInterfaceComponent CreateGenericUserInterfaceComponent(IAnchor anchorDetails);

    }
}
