using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UtilityTypes.BinaryLists
{
    public interface IBinaryListFactory
    {

        IBinaryList<T> CreateEmpty<T>();

    }
}
