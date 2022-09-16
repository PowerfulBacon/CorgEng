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
    internal class ObjectNode : DefinitionNode
    {

        /// <summary>
        /// The type that we want to treat the object as
        /// </summary>
        public Type ReturnType { get; set; }

        /// <summary>
        /// The type that we want to create.
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// The value to apply, unless we contain an object node as a child
        /// </summary>
        public object PropertyValue { get; protected set; }

        public ObjectNode(DefinitionNode parent) : base(parent)
        {
        }

        public override void ParseSelf(XmlNode node)
        {
            //Perform base parsing actions
            base.ParseSelf(node);
            //The type that we want to create
            string typeName = node.Attributes["createdType"]?.Value ?? node.Attributes["type"]?.Value;
            if (!EntityLoader.TypePaths.ContainsKey(typeName))
            {
                throw new ContentLoadException($"Failed to load object node, the type {typeName} does not exist.");
            }
            ObjectType = EntityLoader.TypePaths[typeName];
            //The type that we want to return
            typeName = node.Attributes["returnType"]?.Value;
            if (typeName == null)
            {
                //Return the type that we created.
                ReturnType = ObjectType;
            }
            else
            {
                if (!EntityLoader.TypePaths.ContainsKey(typeName))
                {
                    throw new ContentLoadException($"Failed to load object node, the type {typeName} does not exist.");
                }
                ReturnType = EntityLoader.TypePaths[typeName];
            }
            //Determine our property value
            ParseValue(node);
        }

        protected void ParseValue(XmlNode node)
        {
            //If we are a value type, attempt to directly convert
            if ((!ObjectType.IsGenericType && ObjectType.IsValueType) || ObjectType == typeof(string))
            {
                //Since its a value type, we should be able to just convert it from a string
                PropertyValue = TypeDescriptor.GetConverter(ObjectType).ConvertFrom(node.InnerText.Trim());
            }
        }

        public override object CreateInstance(object parent, Dictionary<string, object> instanceRefs)
        {
            //Create ourself
            object createdObject = PropertyValue ?? Activator.CreateInstance(ObjectType);
            //Store it
            if (Key != null)
            {
                instanceRefs.Add(Key, createdObject);
            }
            //Pass ourself to our children
            foreach (DefinitionNode childNode in Children)
            {
                childNode.CreateInstance(createdObject, instanceRefs);
            }
            //Return the created object
            return createdObject;
        }

    }
}
