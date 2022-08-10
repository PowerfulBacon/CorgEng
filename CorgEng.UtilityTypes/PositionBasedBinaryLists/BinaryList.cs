using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.PositionBasedBinaryLists
{

    internal class BinaryList<T> : IBinaryList<T>
    {

        public delegate bool BinaryListValidityCheckDelegate(T input);

        internal List<BinaryListElement<T>> binaryListElements = new List<BinaryListElement<T>>();

        public T TakeFirst()
        {
            T taken = binaryListElements.First().value;
            binaryListElements.RemoveAt(0);
            return taken;
        }

        public T First()
        {
            return binaryListElements.First().value;
        }

        public int Length()
        {
            return binaryListElements.Count;
        }

        public int Add(int key, T toAdd, int start = 0, int _end = -1)
        {
            //No elements, just add
            if (binaryListElements.Count == 0)
            {
                binaryListElements.Add(new BinaryListElement<T>(key, toAdd));
                return -1;
            }
            //We need to locate the point which is less than the key, and where the key is less than the next value
            //f(i) < key < f(i + 1)
            int end = _end;
            if (end == -1)
                end = binaryListElements.Count - 1;
            //Get the midpoint
            int midPoint = (start + end) / 2;
            //Midpoint has converted
            if (start >= end)
            {
                //Check if the midpoint is too small or too large
                BinaryListElement<T> convergedPoint = binaryListElements.ElementAt(midPoint);
                if (convergedPoint.key > key)
                    binaryListElements.Insert(midPoint, new BinaryListElement<T>(key, toAdd));
                else
                    binaryListElements.Insert(midPoint + 1, new BinaryListElement<T>(key, toAdd));
                return -1;
            }
            //Locate the element at the midpoint
            BinaryListElement<T> current = binaryListElements.ElementAt(midPoint);
            //Perform next search
            if (current.key > key)
                return Add(key, toAdd, start, Math.Max(midPoint - 1, 0));
            else
                return Add(key, toAdd, midPoint + 1, end);
        }

        public void Remove(int key)
        {
            int elementIndex = IndexOf(key);
            if (elementIndex == -1)
                return;
            binaryListElements.RemoveAt(elementIndex);
        }

        public T _ElementAt(int index)
        {
            if (index < 0 || index >= binaryListElements.Count)
                return default(T);
            return binaryListElements.ElementAt(index).value;
        }

        public T ElementWithKey(int key)
        {
            int index = IndexOf(key);
            if (index == -1)
                return default;
            else
                return binaryListElements.ElementAt(index).value;
        }

        private int IndexOf(int i, int start = 0, int _end = -1)
        {
            if (binaryListElements.Count == 0)
                return -1;
            int end = _end;
            if (end == -1)
                end = binaryListElements.Count - 1;
            //Get the midpoint
            int midPoint = (start + end) / 2;
            //Locate the element at the midpoint
            BinaryListElement<T> located = binaryListElements.ElementAt(midPoint);
            //Perform checks
            if (located.key == i)
                return midPoint;
            //Check if we exhausted the list
            if (start >= end)
                return -1;
            //Perform next search
            if (located.key > i)
                return IndexOf(i, start, Math.Max(midPoint - 1, 0));
            else
                return IndexOf(i, midPoint + 1, end);
        }

        public bool ElementsInRange(int min, int max, int start = 0, int _end = -1, BinaryListValidityCheckDelegate<T> conditionalCheck = null)
        {
            //Check the X-Axis
            if (binaryListElements.Count == 0)
                return false;
            int end = _end;
            if (end == -1)
                end = binaryListElements.Count - 1;
            //Get the midpoint
            int midPoint = (start + end) / 2;
            //Locate the element at the midpoint
            BinaryListElement<T> located = binaryListElements.ElementAt(midPoint);
            //Perform checks
            if (located.key >= min && located.key <= max && (conditionalCheck?.Invoke(located.value) ?? true))
                return true;
            //Check if we exhausted the list
            if (start >= end)
                return false;
            //Perform next search
            if (located.key > max)
                return ElementsInRange(min, max, start, Math.Max(midPoint - 1, 0));
            else
                return ElementsInRange(min, max, midPoint + 1, end);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new BinaryListEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BinaryListEnumerator<T>(this);
        }
    }
}
