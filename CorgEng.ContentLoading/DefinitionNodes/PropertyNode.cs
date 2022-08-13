using CorgEng.ContentLoading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// The value to apply, unless we contain an object node as a child
        /// </summary>
        public object PropertyValue { get; protected set; }

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
            //Determine our property value
            ParseValue(node);
        }

        protected void ParseValue(XmlNode node)
        {
            //If we are a value type, attempt to directly convert
            if (TargetProperty.PropertyType.IsValueType || TargetProperty.PropertyType == typeof(string))
            {
                //Since its a value type, we should be able to just convert it from a string
                PropertyValue = TypeDescriptor.GetConverter(TargetProperty.PropertyType).ConvertFrom(node.InnerText.Trim());
            }
        }

        public override object CreateInstance(object parent)
        {
            if (PropertyValue != null)
            {
                TargetProperty.SetValue(parent, PropertyValue);
            }
            else
            {
                ObjectNode childNode = Children[0] as ObjectNode;
                object createdObject = childNode.CreateInstance(null);
                TargetProperty.SetValue(parent, createdObject);
            }
            //Returns nothing
            return null;
        }

    }
}
