using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityQuery
{
    public class EntityQuerySystem : EntitySystem
    {

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.CLIENT_SYSTEM;

        public override void SystemSetup()
        { }
    }
}
