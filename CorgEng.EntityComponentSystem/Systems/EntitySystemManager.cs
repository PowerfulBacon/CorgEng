using CorgEng.Core.Modules;
using CorgEng.Core;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;

namespace CorgEng.EntityComponentSystem.Systems
{

    internal class EntitySystemManager : IEntitySystemManager
    {

        [UsingDependency]
        protected static ILogger Logger;

        /// <summary>
        /// A static list of all entity systems in use
        /// </summary>
        public ConcurrentDictionary<Type, EntitySystem> EntitySystems { get; }
            = new ConcurrentDictionary<Type, EntitySystem>();

        /// <summary>
        /// has setup been completed?
        /// </summary>
        public bool SetupCompleted { get; private set; } = false;

        /// <summary>
        /// Actions to run after setup
        /// </summary>
        public event Action postSetupAction;

        private IWorld world;

        public EntitySystemManager(IWorld world)
        {
            this.world = world;
        }

        ~EntitySystemManager()
        {
            TerminateSubsystems();
            EntitySystems.Clear();
        }

        /// <summary>
        /// Called when the attached world process is created.
        /// Creates all System types and tracks the to prevent GC
        /// </summary>
        internal void CreateAllSystems()
        {
            Logger?.WriteLine($"Setting up systems...", LogType.LOG);
            //Locate all system types using reflection.
            //Note that we need all systems in all loaded modules
            IEnumerable<Type> locatedSystems = CorgEngMain.LoadedAssemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(EntitySystem).IsAssignableFrom(type) && !type.IsAbstract));
            locatedSystems.Select((type) =>
            {
                Logger?.WriteLine($"Initializing {type}...", LogType.LOG);
                EntitySystem createdSystem = (EntitySystem)type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.HasThis, new Type[0], null).Invoke(new object[0]);
                EntitySystems.TryAdd(createdSystem.GetType(), createdSystem);
                return createdSystem;
            })
                // Very important that we fully resolve the enumerator and don't evaluate it lazilly
                .ToList()
                // Now do the foreach after they have all been created
                .ForEach(entitySystem => entitySystem.SystemSetup(world));
            SetupCompleted = true;
            // Run post-setup actions
            postSetupAction?.Invoke();
            postSetupAction = null;
            //Trigger the event when this is all done and loaded
            CorgEngMain.OnReadyEvents += () =>
            {
                new GameReadyEvent().RaiseGlobally();
            };
            Logger?.WriteLine($"Successfully created and setup all systems!", LogType.LOG);
        }

        /// <summary>
        /// Gets a specific entity system
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSingleton<T>()
        {
            return (T)(object)EntitySystems[typeof(T)];
        }

        /// <summary>
        /// Shut down all subsystems that are running inside this entity system
        /// manager.
        /// </summary>
        internal void TerminateSubsystems()
        {
            new GameClosedEvent().RaiseGlobally();
        }

    }
}
