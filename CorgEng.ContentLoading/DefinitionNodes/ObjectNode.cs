﻿using CorgEng.ContentLoading;
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
    internal class ObjectNode : DefinitionNode
    {

        public Type ObjectType { get; set; }

        public ObjectNode(DefinitionNode parent) : base(parent)
        {
        }

        public override void ParseSelf(XmlNode node)
        {
            string typeName = node.Attributes["type"].Value;
            if (!EntityLoader.TypePaths.ContainsKey(typeName))
            {
                throw new ContentLoadException($"Failed to load object node, the type {typeName} does not exist.");
            }
            ObjectType = EntityLoader.TypePaths[typeName];
        }

        public override object CreateInstance(object parent)
        {
            //Create ourself
            object createdObject = Activator.CreateInstance(ObjectType);
            //Pass ourself to our children
            foreach (DefinitionNode childNode in Children)
            {
                childNode.CreateInstance(createdObject);
            }
            //Return the created object
            return createdObject;
        }

    }
}