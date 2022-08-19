using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.PositionBasedBinaryLists
{

    internal class PositionBasedBinaryList<T> : IPositionBasedBinaryList<T>
    {

        private BinaryList<BinaryList<T>> list = new BinaryList<BinaryList<T>>();

        public bool HasElements => list.Length() > 0;

        public T TakeFirst()
        {
            T taken = list.First().TakeFirst();
            if (list.First().Length() == 0)
                list.TakeFirst();
            return taken;
        }

        public T First()
        {
            return list.First().First();
        }

        public void Add(int x, int y, T element)
        {
            //Locate the X element
            BinaryList<T> located = list.ElementWithKey(x);
            //X element contains nothing, initialize it
            if (located == null)
            {
                BinaryList<T> createdList = new BinaryList<T>();
                located = createdList;
                list.Add(x, located);
            }
            //Add the y element
            located.Add(y, element);
        }

        public void Remove(int x, int y)
        {
            //Locate the X element
            BinaryList<T> located = list.ElementWithKey(x);
            //X element contains nothing, initialize it
            if (located == null)
                return;
            located.Remove(y);
            if (located.Length() == 0)
                list.Remove(x);
        }

        public T Get(int x, int y)
        {
            //Locate the X element
            BinaryList<T> located = list.ElementWithKey(x);
            //X element contains nothing, initialize it
            if (located == null)
                return default;
            return located.ElementWithKey(y);
        }

        /// <summary>
        /// Checks if any elements exist within the specified range.
        /// </summary>
        public bool ElementsInRange(int minX, int minY, int maxX, int maxY, int start = 0, int _end = -1, BinaryListValidityCheckDelegate<T> conditionalCheck = null)
        {
            //Check the X-Axis
            if (list.binaryListElements.Count == 0)
                return false;
            int end = _end;
            if (end == -1)
                end = list.binaryListElements.Count - 1;
            //Get the midpoint
            int midPoint = (start + end) / 2;
            //Locate the element at the midpoint
            BinaryListElement<BinaryList<T>> located = list.binaryListElements.ElementAt(midPoint);
            //Perform checks
            if (located.key >= minX && located.key <= maxX)
            {
                if (located.value.ElementsInRange(minY, maxY, 0, -1, conditionalCheck))
                    return true;
            }
            //Check if we exhausted the list
            if (start >= end)
                return false;
            //Perform next search
            if (located.key > maxX)
                return ElementsInRange(minX, minY, maxX, maxY, start, Math.Max(midPoint - 1, 0));
            else
                return ElementsInRange(minX, minY, maxX, maxY, midPoint + 1, end);
        }

        public override string ToString()
        {
            string output = "{\n";
            foreach (BinaryListElement<BinaryList<T>> xAxis in list.binaryListElements)
            {
                int x = xAxis.key;
                foreach (BinaryListElement<T> thing in xAxis.value.binaryListElements)
                {
                    int y = thing.key;
                    if (thing.value is IEnumerable)
                    {
                        output += $"\t({x}, {y}) = (";
                        IEnumerator enumerator = (thing.value as IEnumerable).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            output += $"{enumerator.Current},";
                        }
                        output += $")\n";
                    }
                    else
                        output += $"\t({x}, {y}) = ({thing.value})\n";
                }
            }
            return output + "\n}";
        }

        public void Set(int x, int y, T element)
        {
            //Locate the X element
            BinaryList<T> located = list.ElementWithKey(x);
            //X element contains nothing, initialize it
            if (located == null)
            {
                BinaryList<T> createdList = new BinaryList<T>();
                located = createdList;
                list.Add(x, located);
            }
            //Add the y element
            if (!located.ElementWithKey(y).Equals(default(T)))
            {
                located.Remove(y);
            }
            located.Add(y, element);
        }

    }

}
