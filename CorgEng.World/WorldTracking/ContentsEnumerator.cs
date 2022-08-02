using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.World.WorldTracking
{
    internal class ContentsEnumerator : IEnumerator<IEntity>
    {

        /// <summary>
        /// A reference to the contents holder we are enumerating over
        /// </summary>
        private ContentsHolder reference;

        public ContentsEnumerator(ContentsHolder contentsHolder)
        {
            reference = contentsHolder;
        }

        public IEntity Current => head < reference.contentsArray.Length ? reference.contentsArray[head] : null;

        object IEnumerator.Current => reference.contentsArray[head];

        private int head = -1;

        public void Dispose()
        {
            reference = null;
        }

        public bool MoveNext()
        {
            do
            {
                head++;
                if (head == reference.nextInsertionPointer || head >= reference.contentsArray.Length)
                    return false;
            }
            while (Current == null);
            return true;
        }

        public void Reset()
        {
            head = -1;
        }

    }
}
