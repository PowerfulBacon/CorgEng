using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.World;
using CorgEng.Pathfinding.Queryers;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example.Shared.Components.Gravity
{
    public class SandSystem : ProcessingSystem
    {

        [UsingDependency]
        private static IWorld WorldAccess = default!;

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

        protected override int ProcessDelay => 100;

        /// <summary>
        /// What we use to query if the tile is solid
        /// </summary>
        private WorldPathCellQueryer SolidCellQuery = new WorldPathCellQueryer();

        public override void SystemSetup()
        {
            RegisterLocalEvent<SandComponent, InitialiseEvent>(OnSandInitialise);
            RegisterLocalEvent<SandComponent, DeleteEntityEvent>(OnSandDestroyed);
        }

        private void OnSandInitialise(IEntity entity, SandComponent sandComponent, InitialiseEvent initialiseEvent)
        {
            RegisterProcess<SandComponent>(entity, ProcessSand);
        }

        private void OnSandDestroyed(IEntity entity, SandComponent sandComponent, DeleteEntityEvent deleteEvent)
        {
            StopProcesing(entity);
        }

        private void ProcessSand(IEntity sandEntity, SandComponent sandComponent, double deltaTime)
        {
            TransformComponent sandTransform = sandEntity.GetComponent<TransformComponent>();
            Vector<float> nextPosition = new Vector<float>(sandTransform.Position.X, sandTransform.Position.Y - 1);
            if (SolidCellQuery.EnterPositionCost(nextPosition, GenericInterfaces.Core.Direction.SOUTH) > 0)
            {
                new SetPositionEvent(nextPosition).Raise(sandEntity);
                return;
            }
            //Try to move left
            nextPosition.X -= 1;
            if (SolidCellQuery.EnterPositionCost(nextPosition, GenericInterfaces.Core.Direction.SOUTH_WEST) > 0)
            {
                new SetPositionEvent(nextPosition).Raise(sandEntity);
                return;
            }
            //Try to move right
            nextPosition.X += 2;
            if (SolidCellQuery.EnterPositionCost(nextPosition, GenericInterfaces.Core.Direction.SOUTH_EAST) > 0)
            {
                new SetPositionEvent(nextPosition).Raise(sandEntity);
                return;
            }
        }

    }
}
