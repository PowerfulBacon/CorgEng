using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Config;
using CorgEng.GenericInterfaces.Networking.Serialisation;
using CorgEng.GenericInterfaces.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Components.ComponentVariables.Networking
{

    public static class NetVar
    {

        [UsingDependency]
        internal static INetworkConfig NetworkConfig;

        [UsingDependency]
        internal static ILogger Logger;

        [UsingDependency]
        internal static IAutoSerialiser AutoSerialiser;

        public static HashSet<INetVar> DirtyNetvars = new HashSet<INetVar>();
    }

    /**
     * Networked version of a CVar
     */
    public class NetCVar<TValueType, TComponentType> : CVar<TValueType, TComponentType>, INetVar, ICustomSerialisationBehaviour
        where TComponentType : Component
    {

        /// <summary>
        /// The number of netvars active in the world, start from 1 as 0 represents an
        /// uninitialised netvar.
        /// </summary>
        private static ulong _netVarCount = 1;

        public ulong NetVarID { get; private set; }

        public bool PrototypeSerialised { get; set; }

        public NetCVar()
        {
            if (NetVar.NetworkConfig == null || !NetVar.NetworkConfig.NetworkingActive)
                return;
            if (!NetVar.NetworkConfig.ProcessServerSystems)
            {
                if (!NetVar.NetworkConfig.ProcessClientSystems)
                    throw new Exception("Attempted to initialise a netvar without active networking. This may result in issues.");
                return;
            }
            // Set the NetVarID
            NetVarID = _netVarCount++;
        }

        public NetCVar(TValueType value) : base(value)
        {
            if (NetVar.NetworkConfig == null || !NetVar.NetworkConfig.NetworkingActive)
                return;
            if (!NetVar.NetworkConfig.ProcessServerSystems)
            {
                if (!NetVar.NetworkConfig.ProcessClientSystems)
                    throw new Exception("Attempted to initialise a netvar without active networking. This may result in issues.");
                return;
            }
            // Determine our component type and prepare signal reaction handlers
            NetVarID = _netVarCount++;
        }

        public override void TriggerChanged()
        {
            base.TriggerChanged();
            if (NetVar.NetworkConfig == null || !NetVar.NetworkConfig.NetworkingActive)
                return;
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

        public NetCVar<TValueType, TComponentType> SetPrototypeSerialised(bool value)
        {
            PrototypeSerialised = value;
            return this;
        }

        public object GetValue()
        {
            if (NetVarID == 0)
                throw new Exception("Attempting to access an uninitialised NetVar.");
            return Value;
        }

        public Type GetStoredType()
        {
            return typeof(TValueType);
        }

        public int GetSerialisationLength()
        {
            return NetVar.AutoSerialiser.SerialisationLength(typeof(uint), NetVarID)
                + NetVar.AutoSerialiser.SerialisationLength(typeof(TValueType), Value);
        }

        public void SerialiseInto(BinaryWriter binaryWriter)
        {
            NetVar.AutoSerialiser.SerializeInto(typeof(uint), NetVarID, binaryWriter);
            NetVar.AutoSerialiser.SerializeInto(typeof(TValueType), Value, binaryWriter);
        }

        public void DeserialiseFrom(BinaryReader binaryReader)
        {
            NetVarID = (uint)NetVar.AutoSerialiser.Deserialize(typeof(uint), binaryReader);
            Value = (TValueType)NetVar.AutoSerialiser.Deserialize(typeof(TValueType), binaryReader);
        }
    }
}
