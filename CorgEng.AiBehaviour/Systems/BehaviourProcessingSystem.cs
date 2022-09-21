using CorgEng.AiBehaviour.Components;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.AiBehaviour.Systems
{
    /// <summary>
    /// Processes the mind of pawns.
    /// Decides what actions it should perform.
    /// </summary>
    internal class BehaviourProcessingSystem : ProcessingSystem
    {

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

        protected override int ProcessDelay => 1000;

        public override void SystemSetup()
        {
            RegisterLocalEvent<AiBehaviourComponent, ComponentAddedEvent>(OnComponentAdded);
        }

        private void OnComponentAdded(IEntity entity, AiBehaviourComponent AiBehaviourComponent, ComponentAddedEvent componentAddedEvent)
        {
            //Ignore if its not our component
            if (AiBehaviourComponent != componentAddedEvent.Component)
                return;
            //Start processing
            RegisterProcess<AiBehaviourComponent>(entity, BehaviourProcess);
        }

        /// <summary>
        /// Process event
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="AiBehaviourComponent"></param>
        /// <param name="deltaTime"></param>
        private void BehaviourProcess(IEntity entity, AiBehaviourComponent AiBehaviourComponent, double deltaTime)
        {
            //Currently thinking
            if (AiBehaviourComponent.BehaviourManager.Thinking)
                return;
            //Perform the thinking asynchronously
            AiBehaviourComponent.BehaviourManager.Thinking = true;
            Task.Run(async () =>
            {
                //Perform thinking
                try
                {
                    await AiBehaviourComponent.BehaviourManager.Process();
                }
                catch (Exception e)
                {
                    Logger.WriteLine($"An exception occured while pawn was thinking: {e}", LogType.ERROR);
                }
                //Finish thinking
                AiBehaviourComponent.BehaviourManager.Thinking = false;
            });
        }

    }
}
