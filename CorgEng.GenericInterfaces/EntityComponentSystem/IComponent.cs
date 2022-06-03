using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface IComponent
    {

        bool IsSynced { get; }

        IEntity Parent { get; }

    }

}
