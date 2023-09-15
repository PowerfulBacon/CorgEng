using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.ContentLoading.DefinitionNodes
{
    internal class ArrayNode : DefinitionNode
    {

        //Get the type of the array
        internal Type arrayType;

        public ArrayNode(DefinitionNode parent) : base(parent)
        {
        }

        public override void ParseSelf(XmlNode node)
        {
            //Perform base parsing actions
            base.ParseSelf(node);
            //Get the array type
            arrayType = EntityLoader.TypePaths[node.Attributes["type"].Value];
        }

        public override object CreateInstance(IWorld world, object parent, Dictionary<string, object> instanceRefs)
        {
            //Create the array object
            Array createdArray = Array.CreateInstance(arrayType, Children.Count);
            //Populate the array
            int i = 0;
            foreach (DefinitionNode child in Children)
            {
                createdArray.SetValue(child.CreateInstance(world, createdArray, instanceRefs), i++);
            }
            //Add a reference to the created array
            if (Key != null)
            {
                instanceRefs.Add(Key, createdArray);
            }
            //return the array
            return createdArray;
        }

    }
}
