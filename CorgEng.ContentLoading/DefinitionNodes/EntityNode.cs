using CorgEng.ContentLoading;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes
{
    internal class EntityNode : DefinitionNode, IEntityDefinition
    {

        public bool Abstract { get; set; } = false;

        /// <summary>
        /// A list of the component children that we contain
        /// </summary>
        public List<ComponentNode> ComponentChildren { get; } = new List<ComponentNode>();

        public EntityNode(DefinitionNode parent) : base(parent)
        {
            if (parent != null)
            {
                throw new ContentLoadException("Entity nodes must only be children of Entities nodes.");
            }
        }

        public override void ParseSelf(XmlNode node)
        {
            //Perform base parsing actions
            base.ParseSelf(node);
            // Allow ID to be got from the 'name' attribute from backwards compatability.
            ID = node.Attributes["id"]?.Value ?? node.Attributes["name"]?.Value;
            Abstract = node.Attributes["abstract"]?.Value.ToLower() == "true";
            if (!string.IsNullOrEmpty(ID))
            {
                EntityCreator.EntityNodesByName.Add(ID, this);
            }
        }

        /// <summary>
        /// Create an entity from this node
        /// </summary>
        /// <returns></returns>
        public IEntity CreateEntity(IWorld world, Action<IEntity> setupAction)
        {
            return (IEntity)CreateInstance(world, null, new Dictionary<string, object>(), setupAction);
        }

        public override object CreateInstance(IWorld world, object parent, Dictionary<string, object> instanceRefs)
        {
            return CreateInstance(world, parent, instanceRefs, null);
        }

        public object CreateInstance(IWorld world, object parent, Dictionary<string, object> instanceRefs, Action<IEntity> setupAction)
        {
            IEntity createdEntity = world.EntityManager.CreateUninitialisedEntity();
            //Store the key
            if (Key != null)
            {
                instanceRefs.Add(Key, createdEntity);
            }
            //Add on properties
            foreach (DefinitionNode childNode in Children)
            {
                childNode.CreateInstance(world, createdEntity, instanceRefs);
            }
            //Run init event
            setupAction?.Invoke(createdEntity);
            //Initialise
            new InitialiseEvent().Raise(createdEntity, true);
            return createdEntity;
        }

    }
}
