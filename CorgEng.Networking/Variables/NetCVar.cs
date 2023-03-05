using CorgEng.EntityComponentSystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Variables
{

    internal static class NetVar
    {
        internal static HashSet<INetVar> DirtyNetvars = new HashSet<INetVar>();
    }

    /**
     * Networked version of a CVar
     */
    internal class NetCVar<T> : CVar<T>, INetVar
    {

        private static ulong _netVarCount = 0;

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
            lock (NetVar.DirtyNetvars)
            {
                if (NetVar.DirtyNetvars.Contains(this))
                    return;
                NetVar.DirtyNetvars.Add(this);
            }
        }

        public object GetValue()
        {
            return Value;
        }

        public Type GetStoredType()
        {
            return typeof(T);
        }
    }
}
