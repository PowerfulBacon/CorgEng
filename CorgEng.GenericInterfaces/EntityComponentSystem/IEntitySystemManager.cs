using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface IEntitySystemManager
    {
        
        bool SetupCompleted { get; }

        event Action postSetupAction;

        T GetSingleton<T>();

    }
}
