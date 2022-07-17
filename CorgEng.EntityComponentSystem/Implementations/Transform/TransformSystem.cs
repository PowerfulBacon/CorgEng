using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    public sealed class TransformSystem : EntitySystem
    {

        //Runs only on the server, triggers MoveEvents from position change events.
        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<TransformComponent, SetPositionEvent>(SetEntityPosition);
            RegisterLocalEvent<TransformComponent, TranslateEvent>(TranslateEntity);
        }

        private void TranslateEntity(IEntity entity, TransformComponent transform, TranslateEvent translationEvent)
        {
            Vector<float> newPosition = transform.Position + translationEvent.TranslationDelta;
            //Create a new on move event
            MoveEvent moveEvent = new MoveEvent(transform.Position, newPosition);
            //Update the position of the transform
            transform.Position = newPosition;
            //Trigger the on move event
            moveEvent.Raise(entity);
        }

        private void SetEntityPosition(IEntity entity, TransformComponent transform, SetPositionEvent setPositionEvent)
        {
            //Create a new on move event
            MoveEvent moveEvent = new MoveEvent(transform.Position, setPositionEvent.NewPosition);
            //Update the position of the transform
            transform.Position = setPositionEvent.NewPosition;
            //Trigger the on move event
            moveEvent.Raise(entity);
        }

    }
}
