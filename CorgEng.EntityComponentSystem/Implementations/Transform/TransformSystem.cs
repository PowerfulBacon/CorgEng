using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Systems;
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

        public override void SystemSetup()
        {
            RegisterLocalEvent<TransformComponent, SetPositionEvent>(SetEntityPosition);
            RegisterLocalEvent<TransformComponent, TranslateEvent>(TranslateEntity);
        }

        private void TranslateEntity(Entity entity, TransformComponent transform, TranslateEvent translationEvent)
        {
            Vector<float> newPosition = transform.Position + translationEvent.TranslationDelta;
            //Create a new on move event
            MoveEvent moveEvent = new MoveEvent(transform.Position, newPosition);
            //Update the position of the transform
            transform.Position = newPosition;
            //Trigger the on move event
            moveEvent.Raise(entity);
        }

        private void SetEntityPosition(Entity entity, TransformComponent transform, SetPositionEvent setPositionEvent)
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
