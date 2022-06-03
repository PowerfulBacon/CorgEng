using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.VersionSync;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;

namespace CorgEng.EntityComponentSystem.Components
{

    public abstract class Component : IInstantiatable, IVersionSynced, IComponent
    {

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// The parent of this component
        /// </summary>
        public IEntity Parent { get; internal set; }

        public IEntityDef TypeDef { get; set; }

        // All component types are synced.
        public bool IsSynced { get; } = true;

        public void Initialize(IVector<float> initializePosition)
        { }

        public void PreInitialize(IVector<float> initializePosition)
        { }

        public abstract bool SetProperty(string name, IPropertyDef property);

    }
}
