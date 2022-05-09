using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Core;
using CorgEng.GenericInterfaces.UserInterface.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Rendering
{
    [Dependency]
    internal class UserInterfaceRenderCoreFactory : IUserInterfaceRenderCoreFactory
    {
        public IRenderCore Create()
        {
            return new UserInterfaceRenderCore();
        }
    }
}
