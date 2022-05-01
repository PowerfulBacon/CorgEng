using CorgEng.GenericInterfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Rendering
{
    public interface IUserInterfaceRenderCoreFactory
    {

        IRenderCore Create();

    }
}
