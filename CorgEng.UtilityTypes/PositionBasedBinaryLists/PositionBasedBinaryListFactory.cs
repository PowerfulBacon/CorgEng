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
    internal class PositionBasedBinaryListFactory : IPositionBasedBinaryListFactory
    {
        public IPositionBasedBinaryList<T> CreateEmpty<T>()
        {
            return new PositionBasedBinaryList<T>();
        }
    }
}
