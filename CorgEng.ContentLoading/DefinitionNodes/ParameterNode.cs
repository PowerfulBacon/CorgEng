using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.ContentLoading.DefinitionNodes
{
    internal class ParameterNode : DefinitionNode
    {
        public ParameterNode(DefinitionNode parent) : base(parent)
        {
        }

        public override object CreateInstance(IWorld world, object parent, Dictionary<string, object> instanceRefs)
        {
            object created = Children.First().CreateInstance(world, parent, instanceRefs);
            if (Key != null)
            {
                instanceRefs.Add(Key, created);
            }
            return created;
        }

    }
}
