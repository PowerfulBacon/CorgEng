using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.PositionBasedBinaryLists
{
    internal class BinaryListElement<T>
    {
        public int key;
        public T value;

        public BinaryListElement(int key, T value)
        {
            this.key = key;
            this.value = value;
        }
    }

}
