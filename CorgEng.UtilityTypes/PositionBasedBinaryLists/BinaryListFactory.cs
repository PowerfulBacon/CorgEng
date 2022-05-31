using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.PositionBasedBinaryLists
{
    [Dependency]
    internal class BinaryListFactory : IBinaryListFactory
    {
        public IBinaryList<T> CreateEmpty<T>()
        {
            return new BinaryList<T>();
        }
    }
}
