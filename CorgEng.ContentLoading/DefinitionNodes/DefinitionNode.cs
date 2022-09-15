using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes
{
    internal abstract class DefinitionNode
    {

        public List<DefinitionNode> Children { get; } = new List<DefinitionNode>();

        protected string? Key { get; private set; }

        public string? ID { get; set; }

        public DefinitionNode(DefinitionNode parent)
        {
            parent?.Children.Add(this);
        }

        /// <summary>
        /// Parse the node
        /// </summary>
        /// <param name="node"></param>
        public virtual void ParseSelf(XmlNode node)
        {
            Key = node.Attributes["key"]?.Value;
            // The ID of the node, makes it accessible by other nodes
            ID = node.Attributes["id"]?.Value;
        }

        /// <summary>
        /// Called when the instance needs to be created
        /// </summary>
        /// <returns></returns>
        public abstract object CreateInstance(object parent, Dictionary<string, object> instanceRefs);

    }
}
