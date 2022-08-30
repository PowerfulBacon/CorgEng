using CorgEng.ContentLoading;
using CorgEng.Core.Dependencies;
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

        [UsingDependency]
        public static IEntityFactory EntityFactory;

        public string Name { get; set; }

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
            Name = node.Attributes["name"].Value;
            Abstract = node.Attributes["abstract"]?.Value.ToLower() == "true";
            EntityCreator.EntityNodesByName.Add(Name, this);
        }

        /// <summary>
        /// Create an entity from this node
        /// </summary>
        /// <returns></returns>
        public IEntity CreateEntity()
        {
            return (IEntity)CreateInstance(null, new Dictionary<string, object>());
        }

        public override object CreateInstance(object parent, Dictionary<string, object> instanceRefs)
        {
            IEntity createdEntity = EntityFactory.CreateEmptyEntity();
            //Store the key
            if (Key != null)
            {
                instanceRefs.Add(Key, createdEntity);
            }
            //Add on properties
            foreach (DefinitionNode childNode in Children)
            {
                childNode.CreateInstance(createdEntity, instanceRefs);
            }
            return createdEntity;
        }

    }
}
