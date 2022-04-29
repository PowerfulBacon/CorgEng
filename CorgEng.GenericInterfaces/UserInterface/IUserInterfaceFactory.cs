using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface
{
    public interface IUserInterfaceFactory
    {

        IUserInterface CreateUserInterface();

    }
}
