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

        public DefinitionNode(DefinitionNode parent)
        {
            parent?.Children.Add(this);
        }

        /// <summary>
        /// Parse the node
        /// </summary>
        /// <param name="node"></param>
        public abstract void ParseSelf(XmlNode node);

        /// <summary>
        /// Called when the instance needs to be created
        /// </summary>
        /// <returns></returns>
        public abstract object CreateInstance(object parent);

    }
}
