using CorgEng.AiBehaviour.Components;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.AiBehaviour.Systems
{
    internal class AiActionProcessingSystem : ProcessingSystem
    {

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

        protected override int ProcessDelay => 100;

        public override void SystemSetup(IWorld world)
        {
            RegisterLocalEvent<AiBehaviourComponent, ComponentAddedEvent>(OnComponentAdded);
        }

        private void OnComponentAdded(IEntity entity, AiBehaviourComponent AiBehaviourComponent, ComponentAddedEvent componentAddedEvent)
        {
            if (componentAddedEvent.Component != AiBehaviourComponent)
                return;
            RegisterProcess<AiBehaviourComponent>(entity, OnProcess);
        }

        private void OnProcess(IEntity entity, AiBehaviourComponent AiBehaviourComponent, double deltaTime)
        {
            AiBehaviourComponent.BehaviourManager.CurrentAction?.PerformAction(AiBehaviourComponent.BehaviourManager, deltaTime);
        }

    }
}
