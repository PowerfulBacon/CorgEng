using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.ContentLoading.DefinitionNodes
{
    internal class ElementNode : DefinitionNode
    {

        /// <summary>
        /// The property that we target
        /// </summary>
        public Type ParentType { get; private set; }

        /// <summary>
        /// The value to apply, unless we contain an object node as a child
        /// </summary>
        public object PropertyValue { get; protected set; }

        /// <summary>
        /// Type of the dictionary key, if our parent is a dictionary
        /// </summary>
        private Type dictionaryParentKeyType;

        /// <summary>
        /// Type of the dictionary key, if our parent is a dictionary
        /// </summary>
        private Type dictionaryParentValueType;

        public ElementNode(DefinitionNode parent) : base(parent)
        {
            //Determine return type
            if (parent is ArrayNode array)
            {
                ParentType = array.arrayType;
            }
            else if (parent is DictionaryNode dictionary)
            {
                ParentType = dictionary.keyValuePairType;
                dictionaryParentKeyType = dictionary.keyType;
                dictionaryParentValueType = dictionary.valueType;
            }
            else
            {
                throw new ContentLoadException("Invalid element node, must be a child of an Array or a Dictionary.");
            }
        }

        public override void ParseSelf(XmlNode node)
        {
            //Perform base parsing actions
            base.ParseSelf(node);
            //Determine our property value
            ParseValue(node);
        }

        protected void ParseValue(XmlNode node)
        {
            //If we are a value type, attempt to directly convert
            if ((!ParentType.IsGenericType && ParentType.IsValueType) || ParentType == typeof(string))
            {
                //Since its a value type, we should be able to just convert it from a string
                PropertyValue = TypeDescriptor.GetConverter(ParentType).ConvertFrom(node.InnerText.Trim());
            }
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

        private static MethodInfo cachedMethod = null;

        private object GetValue(object parent, Dictionary<string, object> instanceRefs)
        {
            //Has 2 children, key value pair
            if (Children.Count == 2)
            {
                //Order matters for optimisation sake
                //Select the key
                KeyNode childKey = Children[0] as KeyNode;
                //Select the value
                ValueNode childValue = Children[1] as ValueNode;
                //Return a key value pair with the specified types
                if (cachedMethod == null)
                {
                    cachedMethod = typeof(KeyValuePair).GetMethod("Create", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                }
                //Make the generic method and call it
                return cachedMethod.MakeGenericMethod(dictionaryParentKeyType, dictionaryParentValueType).Invoke(null, new object[] {
                    childKey.CreateInstance(parent, instanceRefs),
                    childValue.CreateInstance(parent, instanceRefs)
                });

            }
            if (Children.Count != 1)
            {
                throw new ContentLoadException("Element node contains invalid children count, it should either contain a single value, or a <Key> and <Value> node.");
            }
            //1 Child, return value
            if (PropertyValue != null)
            {
                return PropertyValue;
            }
            // Either object or value node
            else if (Children[0] is ObjectNode childNode && !(Children[0] is KeyNode))
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
                throw new ContentLoadException("Failed to set array node, invalid value.");
            }
        }

    }
}
