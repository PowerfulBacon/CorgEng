using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.ContentLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example.Components.PlayerMovement
{
    public class PlayerMovementComponent : Component
    {

        public override bool SetProperty(string name, IPropertyDef property)
        {
            return false;
        }

    }
}
