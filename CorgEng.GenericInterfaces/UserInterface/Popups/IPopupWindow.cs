using CorgEng.GenericInterfaces.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Popups
{
    public interface IPopupWindow
    {

        IUserInterfaceComponent AttachedComponent { get; }

        double X { get; set; }

        double Y { get; set; }

        double Width { get; set; }

        double Height { get; set; }

        void ClosePopup();

    }
}
