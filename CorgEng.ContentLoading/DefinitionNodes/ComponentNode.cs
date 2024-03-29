﻿using CorgEng.ContentLoading;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes
{
    internal class ComponentNode : ObjectNode
    {

        EntityNode Parent { get; }

        public ComponentNode(DefinitionNode parent) : base(parent)
        {
            Parent = parent as EntityNode;
        }

        public override object CreateInstance(IWorld world, object parent, Dictionary<string, object> instanceRefs)
        {
            //Create the component
            IComponent createdComponent = base.CreateInstance(world, parent, instanceRefs) as IComponent;
            //Add the compoennt
            IEntity parentEntity = parent as IEntity;
            parentEntity.AddComponent(createdComponent);
            return createdComponent;
        }
    }
}
