using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UtilityTypes
{
    public interface IListenable
    {

        //Callback for when the value is changed
        event EventHandler ValueChanged;

    }
}
