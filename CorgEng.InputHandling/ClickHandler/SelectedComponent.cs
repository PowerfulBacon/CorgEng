using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.ClickHandler
{
    internal class SelectedComponent : IComponent
    {

        public bool IsSynced => false;

        public IEntity Parent { get; set; }

    }
}
