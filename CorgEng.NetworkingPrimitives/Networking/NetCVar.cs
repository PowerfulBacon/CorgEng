using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.NetworkingPrimitives.Variables
{

    internal static class NetVar
    {
        internal static HashSet<INetVar> DirtyNetvars = new HashSet<INetVar>();
    }

    /**
     * Networked version of a CVar
     */
    public class NetCVar<T> : CVar<T>, INetVar
    {

        private static ulong _netVarCount = 0;

        public ulong NetVarID { get; private set; }

        public bool PrototypeSerialised { get; set; }

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

        public NetCVar<T> SetPrototypeSerialised(bool value)
        {
            PrototypeSerialised = value;
            return this;
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
