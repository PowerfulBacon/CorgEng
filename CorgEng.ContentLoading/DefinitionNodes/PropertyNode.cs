using CorgEng.ContentLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes
{
    internal class PropertyNode : DefinitionNode
    {

        public ObjectNode ObjectParent { get; }

        /// <summary>
        /// The property that we target
        /// </summary>
        public PropertyInfo TargetProperty { get; private set; }

        public PropertyNode(DefinitionNode parent) : base(parent)
        {
            ObjectParent = parent as ObjectNode;
            if (ObjectParent == null)
            {
                throw new ContentLoadException("A property node is attached to a non object/component node.");
            }
        }

        public override void ParseSelf(XmlNode node)
        {
            //Get the property that we effect
            TargetProperty = ObjectParent.ObjectType.GetProperty(node.Attributes["name"].Value);
        }

        public override object CreateSelf(object createdParent)
        {
            
        }
    }
}
