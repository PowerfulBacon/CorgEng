using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Attributes
{
    public class UserInterfaceCodeCallbackAttribute : Attribute
    {

        public string KeyName { get; set; }

        public UserInterfaceCodeCallbackAttribute(string keyName)
        {
            KeyName = keyName;
        }
    }
}
