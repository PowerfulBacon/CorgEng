using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
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

        public override object CreateInstance(object parent, Dictionary<string, object> instanceRefs)
        {
            object created = Children.First().CreateInstance(parent, instanceRefs);
            if (Key != null)
            {
                instanceRefs.Add(Key, created);
            }
            return created;
        }

    }
}
