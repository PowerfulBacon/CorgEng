using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityQuery
{

    public class EntityQuery
    {
        [UsingDependency]
        internal static ILogger Logger = default!;
    }

    public class EntityQuery<TComponent> : EntityQuery
        where TComponent : Component
    {

        /// <summary>
        /// Static list of entity queries that currently exist, for optimisation
        /// </summary>
        private static Dictionary<Type, EntityQuery> EntityQueryLookup = new Dictionary<Type, EntityQuery>();

        /// <summary>
        /// Query that is attached to us
        /// </summary>
        private EntityQuery<TComponent>? _parentQuery = null;

        /// <summary>
        /// The components that we have located/are tracking.
        /// </summary>
        private HashSet<TComponent> components = new HashSet<TComponent>();

        /// <summary>
        /// Fetch the query if it already exists
        /// </summary>
        public EntityQuery()
        {
            // Lock the entity query lookup so that we don't have issues with threading.
            lock (EntityQueryLookup)
            {
                // Have a look and see if the entity query exists
                if (EntityQueryLookup.TryGetValue(typeof(TComponent), out EntityQuery? value))
                {
                    _parentQuery = (EntityQuery<TComponent>)value;
                    return;
                }
                EntityQueryLookup.Add(typeof(TComponent), this);
                Action setupFunction = () =>
                    {
                        // The entity lookup doesn't exist, perform an expensive search over all entities
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                        components = EntityManager.GetEntityArrayUnsafe()
                            .Select(entity => entity?.FindComponent<TComponent>())
                            .Where(component => component != null)
                            .ToHashSet();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                        if (components.Count > 0)
                        {
                            EntityQuery.Logger.WriteLine($"Performed an entity query locating {components.Count} components. " +
                                $"It would be far more efficient to initialise the query at runtime.", LogType.WARNING);
                        }
                        // Force ourselves into the entity query system
                        EntitySystem.GetSingleton<EntityQuerySystem>()
                            .RegisterLocalEvent<TComponent, ComponentAddedEvent>((entity, component, signal) =>
                            {
                                if (component != signal.Component)
                                    return;
                                components.Add(component);
                            });
                        EntitySystem.GetSingleton<EntityQuerySystem>()
                            .RegisterLocalEvent<TComponent, ComponentRemovedEvent>((entity, component, signal) =>
                            {
                                if (component != signal.Component)
                                    return;
                                components.Remove(component);
                            });
                    };
                if (EntitySystem.SetupCompleted)
                    setupFunction();
                else
                    EntitySystem.postSetupAction += setupFunction;
            }
        }

        public IEnumerable<TComponent> GetAll()
        {
            return components;
        }

    }
}
