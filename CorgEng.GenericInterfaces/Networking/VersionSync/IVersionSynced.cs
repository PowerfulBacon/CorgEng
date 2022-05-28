using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.VersionSync
{
    public interface IVersionSynced
    {

        /// <summary>
        /// Is this entity version synced?
        /// </summary>
        bool IsSynced { get; }

    }
}
