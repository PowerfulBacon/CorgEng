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
    internal class InstanceNode : DefinitionNode
    {

        private string referenceName;

        public InstanceNode(DefinitionNode parent) : base(parent)
        {
        }

        public override void ParseSelf(XmlNode node)
        {
            //Do generic parsing
            base.ParseSelf(node);
            //Get reference name
            referenceName = node.InnerText.Trim();
        }

        public override object CreateInstance(IWorld world, object parent, Dictionary<string, object> instanceRefs)
        {
            if (instanceRefs.ContainsKey(referenceName))
            {
                return instanceRefs[referenceName];
            }
            else
            {
                return EntityLoader.GetDefinition(referenceName).CreateInstance(world, parent, instanceRefs);
            }
        }

    }
}
