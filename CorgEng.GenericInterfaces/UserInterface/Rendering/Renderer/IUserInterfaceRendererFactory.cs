using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Rendering.Renderer
{
    public interface IUserInterfaceRendererFactory
    {

        IUserInterfaceRenderer CreateUserInterfaceRenderer();

    }
}
