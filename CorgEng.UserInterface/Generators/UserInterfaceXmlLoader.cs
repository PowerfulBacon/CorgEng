using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UserInterface.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Generators
{
    [Dependency]
    public class UserInterfaceXmlLoader : IUserInterfaceXmlLoader
    {

        public IUserInterfaceComponent LoadUserInterface(string filepath)
        {
            return null;
        }

    }
}
