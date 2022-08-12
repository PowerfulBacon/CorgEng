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

        public string Name { get; set; }

        public bool Abstract { get; set; } = false;

        public EntityNode(DefinitionNode parent) : base(parent)
        {
        }

        public override void ParseSelf(XmlNode node)
        {
            Name = node.Attributes["name"].Value;
            Abstract = node.Attributes["abstract"]?.Value.ToLower() == "true";
        }

        /// <summary>
        /// Create an entity from this node
        /// </summary>
        /// <returns></returns>
        public IEntity CreateEntity()
        {
            throw new NotImplementedException();
        }
    }
}
