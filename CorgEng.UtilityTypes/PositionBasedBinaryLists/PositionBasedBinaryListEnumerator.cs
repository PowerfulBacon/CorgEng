using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.PositionBasedBinaryLists
{
    internal class PositionBasedBinaryListEnumerator<T> : IEnumerator<T>
    {

        private readonly PositionBasedBinaryList<T> _list;

        public PositionBasedBinaryListEnumerator(PositionBasedBinaryList<T> list)
        {
            _list = list;
        }

        private int _listIndex = 0;
        private int _index = -1;

        public T Current => _list.list.binaryListElements[_listIndex].value.binaryListElements[_index].value;

        object IEnumerator.Current => _list.list.binaryListElements[_listIndex].value.binaryListElements[_index].value;

        public void Dispose()
        { }

        public bool MoveNext()
        {
            _index++;
            if (_index >= _list.list.binaryListElements[_listIndex].value.binaryListElements.Count)
            {
                _listIndex++;
                _index = 0;
            }
            return _listIndex < _list.list.binaryListElements.Count;
        }

        public void Reset()
        {
            _index = -1;
            _listIndex = 0;
        }

    }
}
