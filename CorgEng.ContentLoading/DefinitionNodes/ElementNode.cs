using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.ContentLoading.DefinitionNodes
{
    internal class ElementNode : DefinitionNode
    {

        /// <summary>
        /// The value to apply, unless we contain an object node as a child
        /// </summary>
        public object PropertyValue { get; protected set; }

        public ElementNode(DefinitionNode parent) : base(parent)
        {
        }

        public override void ParseSelf(XmlNode node)
        {
            //Perform base parsing actions
            base.ParseSelf(node);
        }

        public override object CreateInstance(object parent, Dictionary<string, object> instanceRefs)
        {
            object propertyValue = GetValue(parent, instanceRefs);
            if (Key != null)
            {
                instanceRefs.Add(Key, propertyValue);
            }
            return propertyValue;
        }

        private object GetValue(object parent, Dictionary<string, object> instanceRefs)
        {
            if (PropertyValue != null)
            {
                return PropertyValue;
            }
            else if (Children[0] is ObjectNode childNode)
            {
                object createdObject = childNode.CreateInstance(null, instanceRefs);
                return createdObject;
            }
            else if (Children[0] is DependencyNode dependencyNode)
            {
                object createdObject = dependencyNode.CreateInstance(null, instanceRefs);
                return createdObject;
            }
            else
            {
                throw new Exception("Failed to set array node, invalid value.");
            }
        }

    }
}
