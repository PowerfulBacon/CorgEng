using CorgEng.Audio.Components;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UtilityTypes;
using GJ2022.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Audio.Systems
{
    internal class AudioListenerSystem : EntitySystem
    {

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.CLIENT_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<AudioListenerComponent, MoveEvent>(AudioListenerMoved);
            RegisterLocalEvent<AudioListenerComponent, InitialiseEvent>(AudioListenerInitialised);
            RegisterLocalEvent<AudioListenerComponent, ComponentRemovedEvent>(ComponentRemoved);
        }

        private void AudioListenerMoved(IEntity entity, AudioListenerComponent audioListenerComponent, MoveEvent moveEvent)
        {
            AudioMaster.UpdateListener(moveEvent.NewPosition.X, moveEvent.NewPosition.Y, 0);
        }

        private void AudioListenerInitialised(IEntity entity, AudioListenerComponent audioListenerComponent, InitialiseEvent initialiseEvent)
        {
            IVector<float> position = audioListenerComponent.Transform.Position;
            AudioMaster.UpdateListener(position.X, position.Y, 0);
        }

        private void ComponentRemoved(IEntity entity, AudioListenerComponent audioListenerComponent, ComponentRemovedEvent componentRemovedEvent)
        {
            // Transform was removed
            if (componentRemovedEvent.Component is TransformComponent)
            {
                audioListenerComponent.Transform = null!;
            }
        }

    }
}
