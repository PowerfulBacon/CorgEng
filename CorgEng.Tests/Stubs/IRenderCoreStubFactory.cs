using CorgEng.GenericInterfaces.Core;
using CorgEng.GenericInterfaces.UserInterface.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.Stubs
{
    internal class IRenderCoreStubFactory : IUserInterfaceRenderCoreFactory
    {
        public IRenderCore Create()
        {
            return new RenderCoreStub();
        }
    }
}
