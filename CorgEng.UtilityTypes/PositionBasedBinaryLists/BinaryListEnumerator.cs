using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.PositionBasedBinaryLists
{
    internal class BinaryListEnumerator<T> : IEnumerator<T>
    {

        private readonly BinaryList<T> _list;

        public BinaryListEnumerator(BinaryList<T> list)
        {
            _list = list;
        }

        private int _index = -1;

        public T Current => _list.binaryListElements[_index].value;

        object IEnumerator.Current => _list.binaryListElements[_index].value;

        public void Dispose()
        { }

        public bool MoveNext()
        {
            _index++;
            return _index < _list.binaryListElements.Count;
        }

        public void Reset()
        {
            _index = -1;
        }

    }
}
