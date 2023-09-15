using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface IComponentSignalInjector
    {

        void OnComponentAdded(IComponent component, IEntity parent);

        void OnComponentRemoved(IComponent component, IEntity parent, bool silent);

    }
}
