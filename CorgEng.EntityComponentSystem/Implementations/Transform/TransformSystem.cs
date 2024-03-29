﻿using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
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
        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM | EntitySystemFlags.CLIENT_SYSTEM;

        [UsingDependency]
        private static ILogger Logger;

        public override void SystemSetup(IWorld world)
        {
            RegisterLocalEvent<TransformComponent, SetPositionEvent>(SetEntityPosition);
            RegisterLocalEvent<TransformComponent, TranslateEvent>(TranslateEntity);
            RegisterLocalEvent<TransformComponent, SetRotationEvent>(SetEntityRotation);
            RegisterLocalEvent<TransformComponent, MoveEvent>((e, c, s) => {
                c.Position.Value = s.NewPosition;
            });
        }

        /// <summary>
        /// Set an entity's rotation to some value
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transform"></param>
        /// <param name="setRotationEvent"></param>
        private void SetEntityRotation(IEntity entity, TransformComponent transform, SetRotationEvent setRotationEvent)
        {
            RotationEvent rotationEvent = new RotationEvent(transform.Rotation.Value, setRotationEvent.NewRotation);
            //Update the rotation of the transform component
            transform.Rotation.Value = setRotationEvent.NewRotation;
            //Raise a rotationEvent
            rotationEvent.Raise(entity);
        }

        private void TranslateEntity(IEntity entity, TransformComponent transform, TranslateEvent translationEvent)
        {
            Vector<float> newPosition = transform.Position.Value + translationEvent.TranslationDelta;
            //Create a new on move event
            MoveEvent moveEvent = new MoveEvent(transform.Position.Value, newPosition);
            //Update the position of the transform
            transform.Position.Value = newPosition;
            //Trigger the on move event
            moveEvent.Raise(entity);
        }

        private void SetEntityPosition(IEntity entity, TransformComponent transform, SetPositionEvent setPositionEvent)
        {
            //Create a new on move event
            MoveEvent moveEvent = new MoveEvent(transform.Position.Value, setPositionEvent.NewPosition);
            //Update the position of the transform
            transform.Position.Value = setPositionEvent.NewPosition;
            //Trigger the on move event
            moveEvent.Raise(entity);
        }

    }
}
