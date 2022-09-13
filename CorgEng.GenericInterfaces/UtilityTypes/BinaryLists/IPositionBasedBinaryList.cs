using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UtilityTypes.BinaryLists
{
    public interface IPositionBasedBinaryList<T> : IEnumerable<T>
    {

        bool HasElements { get; }

        T TakeFirst();

        T First();

        void Add(int x, int y, T element);

        void Remove(int x, int y);

        void Set(int x, int y, T element);

        T Get(int x, int y);

        bool ElementsInRange(int minX, int minY, int maxX, int maxY, int start = 0, int _end = -1, BinaryListValidityCheckDelegate<T> conditionalCheck = null);

    }
}
