using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.UserInterface.Components;
using CorgEng.UserInterface.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Systems
{
    public class UserInterfaceClickSystem : EntitySystem
    {

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.CLIENT_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<UserInterfaceClickerComponent, UserInterfaceClickEvent>(OnUserInterfaceElementClicked);
            RegisterLocalEvent<UserInterfaceClickActionComponent, UserInterfaceClickEvent>(OnUserInterfaceActionElementClicked);
        }

        private void OnUserInterfaceElementClicked(IEntity entity, UserInterfaceClickerComponent clickerComponent, UserInterfaceClickEvent userInterfaceClickEvent)
        {
            clickerComponent.InvokationMethod.Invoke(null, null);
        }

        private void OnUserInterfaceActionElementClicked(IEntity entity, UserInterfaceClickActionComponent clickerComponent, UserInterfaceClickEvent userInterfaceClickEvent)
        {
            clickerComponent.InvokationAction.Invoke();
        }

    }
}
