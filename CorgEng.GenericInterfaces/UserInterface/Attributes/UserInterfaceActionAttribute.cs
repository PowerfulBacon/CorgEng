using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Attributes
{
    /// <summary>
    /// Attribute to allow for user interfaces to hook onto certain code functions.
    /// These will be cached on launch for performance.
    /// 
    /// <ButtonComponent OnClick="PurchaseItem" />
    /// 
    /// [UserInterfaceActionAttribute("PurchaseItem")]
    /// public static void PurchaseItem(UserInterfaceAction action)
    /// {
    ///     ...
    /// }
    /// </summary>
    public class UserInterfaceActionAttribute : Attribute
    {

        public string Name { get; set; }

    }
}
