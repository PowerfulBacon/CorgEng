using CorgEng.Contents.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Contents.Systems
{
    internal class ContentsSystem : EntitySystem
    {

        /// <summary>
        /// Function that creates a contents component, adds it to the parent and then returns the created component.
        /// </summary>
        private static Func<IEntity, ContentsComponent> CreateAndAddContentsFunction = (parent) =>
        {
            ContentsComponent createdComponent = new ContentsComponent();
            parent.AddComponent(createdComponent);
            return createdComponent;
        };

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup(IWorld world)
        {
            RegisterLocalEvent<ContainedComponent, DeleteEntityEvent>(OnContainedEntityDeleted);
            RegisterLocalEvent<ContentsComponent, DeleteEntityEvent>(OnContentHolderDeleted);
            RegisterLocalEvent<ContainedComponent, RemoveFromContentsEvent>(ExitContainer);
            RegisterLocalEvent<DeleteableComponent, PutInsideContentsEvent>(EnterContainer);
        }

        /// <summary>
        /// Causes an entity to exit its container
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="deleteableComponent"></param>
        /// <param name="removeFromContentsEvent"></param>
        private void ExitContainer(IEntity entity, ContainedComponent containedComponent, RemoveFromContentsEvent removeFromContentsEvent)
        {
            ContentsComponent parentContents = containedComponent.Parent.GetComponent<ContentsComponent>();
            parentContents.EntitiesInContents.Remove(entity);
            //Parent contains nothing, remove the contents component
            if (parentContents.EntitiesInContents.Count == 0)
            {
                containedComponent.Parent.RemoveComponent(parentContents, false);
            }
            //Set the position of the thing that left the contents, if needed
            TransformComponent? locatedTransform = containedComponent.Parent.FindComponent<TransformComponent>();
            if (locatedTransform != null)
            {
                new SetPositionEvent(locatedTransform.Position.Value).Raise(entity);
            }
            //Raise the left contents event
            new ContentsChangedEvent(containedComponent.Parent, null).Raise(entity);
        }

        /// <summary>
        /// Causes an entity to enter the container of another entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="deleteableComponent"></param>
        /// <param name="putInsideContentsEvent"></param>
        private void EnterContainer(IEntity entity, DeleteableComponent deleteableComponent, PutInsideContentsEvent putInsideContentsEvent)
        {
            //Check if we are stored inside something
            ContainedComponent? containedComponent = entity.FindComponent<ContainedComponent>();
            //We left the contents of an entity
            if (containedComponent != null)
            {
                //Perform leaving actions
                new ContentsChangedEvent(containedComponent.Parent, putInsideContentsEvent.NewHolder).Raise(entity);
                //Set the contained component's parent
                containedComponent.Parent = putInsideContentsEvent.NewHolder;
            }
            else
            {
                //Perform leaving actions
                new ContentsChangedEvent(null, putInsideContentsEvent.NewHolder).Raise(entity);
                //Add the contained component
                containedComponent = new ContainedComponent(putInsideContentsEvent.NewHolder);
                entity.AddComponent(containedComponent);
            }
            //If necessary, add a contents component to the parent
            ContentsComponent parentContentsComponent = putInsideContentsEvent.NewHolder.FindComponent<ContentsComponent>()
                ?? CreateAndAddContentsFunction.Invoke(putInsideContentsEvent.NewHolder);
            //Add the entity to the container
            parentContentsComponent.EntitiesInContents.Add(entity);
        }

        /// <summary>
        /// When something stored inside another object is deleted.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="containedComponent"></param>
        /// <param name="deleteEntityEvent"></param>
        private void OnContainedEntityDeleted(IEntity entity, ContainedComponent containedComponent, DeleteEntityEvent deleteEntityEvent)
        {
            ContentsComponent parentContents = containedComponent.Parent.GetComponent<ContentsComponent>();
            parentContents.EntitiesInContents.Remove(entity);
            //Parent contains nothing, remove the contents component
            if (parentContents.EntitiesInContents.Count == 0)
            {
                containedComponent.Parent.RemoveComponent(parentContents, false);
            }
        }

        /// <summary>
        /// If an entity with children is deleted, all the children should be deleted too.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="contentsComponent"></param>
        /// <param name="deleteEntityEvent"></param>
        private void OnContentHolderDeleted(IEntity entity, ContentsComponent contentsComponent, DeleteEntityEvent deleteEntityEvent)
        {
            switch (contentsComponent.DestroyReaction)
            {
                case ParentDestroyReaction.DESTROY_CHILDREN:
                    foreach (IEntity childEntity in contentsComponent.EntitiesInContents)
                    {
                        new DeleteEntityEvent().Raise(childEntity);
                    }
                    return;
                case ParentDestroyReaction.DROP_CHILDREN:
                    TransformComponent? parentTransform = entity.FindComponent<TransformComponent>();
                    if (parentTransform != null)
                    {
                        //Leave the contents
                        ContentsChangedEvent contentsExitedEvent = new ContentsChangedEvent(entity, null);
                        //Set the new position to the dropped location
                        foreach (IEntity childEntity in contentsComponent.EntitiesInContents)
                        {
                            contentsExitedEvent.Raise(childEntity);
                        }
                    }
                    //Leave the contents
                    return;
            }
        }

    }
}
