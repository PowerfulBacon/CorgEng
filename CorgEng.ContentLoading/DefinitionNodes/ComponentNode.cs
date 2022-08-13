using CorgEng.ContentLoading;
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
            if (Parent == null)
            {
                throw new ContentLoadException("Component nodes must be children of an entity node.");
            }
        }

        public override object CreateInstance(object parent)
        {
            IComponent createdComponent = base.CreateInstance(parent) as IComponent;

            IEntity parentEntity = parent as IEntity;
            parentEntity.AddComponent(createdComponent);
            return createdComponent;
        }
    }
}
