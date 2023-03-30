using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.ContentLoading.DefinitionNodes
{
    internal class DictionaryNode : DefinitionNode
    {

        //Get the type of the array
        internal Type keyType;
        internal Type valueType;

        private Type dictionaryType;

        internal Type keyValuePairType;

        private MethodInfo addMethod;

        public DictionaryNode(DefinitionNode parent) : base(parent)
        {
        }

        public override void ParseSelf(XmlNode node)
        {
            //Perform base parsing actions
            base.ParseSelf(node);
            //Get the array type
            keyType = EntityLoader.TypePaths[node.Attributes["keyType"].Value];
            valueType = EntityLoader.TypePaths[node.Attributes["valueType"].Value];
            dictionaryType = typeof(Dictionary<object, object>).GetGenericTypeDefinition().MakeGenericType(keyType, valueType);
            keyValuePairType = typeof(KeyValuePair<object, object>).GetGenericTypeDefinition().MakeGenericType(keyType, valueType);
            Type iCollectionType = typeof(ICollection<object>).GetGenericTypeDefinition().MakeGenericType(keyValuePairType);
            addMethod = iCollectionType.GetMethod(
                "Add",
                BindingFlags.Public | BindingFlags.Instance);
        }

        public override object CreateInstance(IWorld world, object parent, Dictionary<string, object> instanceRefs)
        {
            //Create the array object
            object dictionary = Activator.CreateInstance(dictionaryType);
            //Populate the dictionary
            foreach (DefinitionNode child in Children)
            {
                addMethod.Invoke(dictionary, new object[] {
                    child.CreateInstance(world, dictionary, instanceRefs)
                });
            }
            //Add a reference to the created array
            if (Key != null)
            {
                instanceRefs.Add(Key, dictionary);
            }
            //return the array
            return dictionary;
        }

    }
}
