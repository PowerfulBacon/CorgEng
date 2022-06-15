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

        public IEntity Current => reference.contentsArray[head];

        object IEnumerator.Current => reference.contentsArray[head];

        private int head = 0;

        public void Dispose()
        {
            reference = null;
        }

        public bool MoveNext()
        {
            head++;
            if (head == reference.nextInsertionPointer)
                return false;
            return true;
        }

        public void Reset()
        {
            head = 0;
        }

    }
}
