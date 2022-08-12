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

        /// <summary>
        /// A list of our children
        /// </summary>
        internal List<DefinitionNode> Children { get; } = new List<DefinitionNode>();

        internal DefinitionNode Parent { get; }

        public DefinitionNode(DefinitionNode parent)
        {
            //Set the parent node
            Parent = parent;
        }

        /// <summary>
        /// Parse the node
        /// </summary>
        /// <param name="node"></param>
        public abstract void ParseSelf(XmlNode node);

        /// <summary>
        /// Spawn this node.
        /// </summary>
        /// <param name="createdParent"></param>
        public void CreateInstance(object createdParent)
        {
            object createdThing = CreateSelf(createdParent);
            foreach (DefinitionNode childNode in Children)
            {
                childNode.CreateInstance(createdThing);
            }
        }

        public abstract object CreateSelf(object createdParent);

    }
}
