using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UtilityTypes
{
    public interface IVector<T>
    {

        event EventHandler OnChange;

        T this[int x] { get; set; }

        T X { get; set; }

        T Y { get; set; }
        
        T Z { get; set; }

        int Dimensions { get; }

    }
}
