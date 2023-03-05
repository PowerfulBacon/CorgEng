using CorgEng.EntityComponentSystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Variables
{
    /**
     * Networked version of a CVar
     */
    internal class NetCVar<T> : CVar<T>, INetVar
    {

        private static ulong _netVarCount = 0;

        internal static HashSet<INetVar> DirtyNetvars = new HashSet<INetVar>();

        public ulong NetVarID { get; private set; }

        public NetCVar()
        {
            // Set the NetVarID
            NetVarID = _netVarCount++;
        }

        public override void TriggerChanged()
        {
            base.TriggerChanged();
            MarkDirty();
        }

        public void MarkDirty()
        {
            lock (DirtyNetvars)
            {
                if (DirtyNetvars.Contains(this))
                    return;
                DirtyNetvars.Add(this);
            }
        }

    }
}
