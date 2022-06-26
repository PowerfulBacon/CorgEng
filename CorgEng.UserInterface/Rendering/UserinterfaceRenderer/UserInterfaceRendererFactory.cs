using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UserInterface.Rendering.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer
{
    [Dependency]
    internal sealed class UserInterfaceRendererFactory : IUserInterfaceRendererFactory
    {
        public IUserInterfaceRenderer CreateUserInterfaceRenderer(uint networkedIdentifier)
        {
            return new UserInterfaceRenderer(networkedIdentifier);
        }
    }
}
