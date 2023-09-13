using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.EntityQuery;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.InputHandler;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.World;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example.Shared.Components.FollowMouseComponent
{
    public class FollowCursorComponent : Component
    {
    }

    internal class FollowCursorSystem : EntitySystem
    {

        [UsingDependency]
        private static IInputHandler InputHandler = null!;

        [UsingDependency]
        private static IEntityPositionTracker World;

        private static EntityQuery<FollowCursorComponent> CursorComponents;

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.CLIENT_SYSTEM;

        public override void SystemSetup(IWorld world)
        {
            CursorComponents = new EntityQuery<FollowCursorComponent>(world);
            InputHandler.AddMouseMoveBind("cursor_moved");
            InputHandler.AddMouseMoveAction("cursor_moved", (deltaTime, mouseX, mouseY) => {
                Vector<float> worldCoordinates = (Vector<float>)CorgEngMain.MainCamera.ScreenToWorldCoordinates(mouseX * 2 - 1, mouseY * 2 - 1, CorgEngMain.GameWindow.Width, CorgEngMain.GameWindow.Height);
                foreach (FollowCursorComponent followCursorComponent in CursorComponents.GetAll())
                {
                    new SetPositionEvent(worldCoordinates).Raise(followCursorComponent.Parent);
                }
                return false;
            }, 1000);
        }

    }
}
