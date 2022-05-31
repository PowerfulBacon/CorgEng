using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UtilityTypes.BinaryLists
{

    public delegate bool BinaryListValidityCheckDelegate<T>(T input);

    public interface IBinaryList<T>
    {

        T TakeFirst();

        T First();

        int Length();

        int Add(int key, T toAdd, int start = 0, int _end = -1);

        void Remove(int key);

        T _ElementAt(int index);

        T ElementWithKey(int key);

        bool ElementsInRange(int min, int max, int start = 0, int _end = -1, BinaryListValidityCheckDelegate<T> conditionalCheck = null);

    }
}
