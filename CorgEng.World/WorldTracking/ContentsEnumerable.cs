using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.World;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.World.WorldTracking
{
    internal class ContentsEnumerable : IEnumerable<IWorldTrackComponent>
    {

        private ContentsHolder holder;

        public ContentsEnumerable(ContentsHolder parent)
        {
            holder = parent;
        }

        public IEnumerator<IWorldTrackComponent> GetEnumerator()
        {
            return new ContentsEnumerator(holder);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ContentsEnumerator(holder);
        }

    }
}
