﻿using CorgEng.ContentLoading;
using CorgEng.ContentLoading.DefinitionNodes;
using CorgEng.GenericInterfaces.EntityComponentSystem;
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
            //Perform base parsing actions
            base.ParseSelf(node);
            //Get the property that we effect
            TargetProperty = ObjectParent.ObjectType.GetProperty(node.Attributes["name"].Value);
            if (TargetProperty == null)
            {
                throw new ContentLoadException($"Could not locate the property {node.Attributes["name"].Value} o the type {ObjectParent.ObjectType}");
            }
            //Determine our property value
            ParseValue(node);
        }

        protected void ParseValue(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode is XmlElement)
                    return;
            }
            //Is enum
            if (TargetProperty.PropertyType.IsEnum)
            {
                PropertyValue = Enum.Parse(TargetProperty.PropertyType, node.InnerText.Trim());
            }
            //If we are a value type, attempt to directly convert
            else if (TargetProperty.PropertyType.IsValueType || TargetProperty.PropertyType == typeof(string))
            {
                //Since its a value type, we should be able to just convert it from a string
                PropertyValue = TypeDescriptor.GetConverter(TargetProperty.PropertyType).ConvertFrom(node.InnerText.Trim());
            }
        }

        public override object CreateInstance(IWorld world, object parent, Dictionary<string, object> instanceRefs)
        {
            if (PropertyValue != null)
            {
                TargetProperty.SetValue(parent, PropertyValue);
                if (Key != null)
                {
                    instanceRefs.Add(Key, PropertyValue);
                }
            }
            else
            {
                object createdObject = Children[0].CreateInstance(world, null, instanceRefs);
                if (Key != null)
                {
                    instanceRefs.Add(Key, createdObject);
                }
                TargetProperty.SetValue(parent, createdObject);
            }
            //Returns nothing
            return null;
        }

    }
}
