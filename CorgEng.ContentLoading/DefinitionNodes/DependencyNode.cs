using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
using CorgEng.GenericInterfaces.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.ContentLoading.DefinitionNodes
{
    internal class DependencyNode : DefinitionNode
    {

        [UsingDependency]
        public static IDependencyFetcher DependencyFetcher;

        /// <summary>
        /// The created dependency object
        /// </summary>
        private object dependencyObject;

        public DependencyNode(DefinitionNode parent) : base(parent)
        {
        }

        public override void ParseSelf(XmlNode node)
        {
            //Get the type name
            string typeName = node.Attributes["type"].Value;
            //The type
            Type dependencyType = EntityLoader.TypePaths[typeName];
            //Get the dependency object from the dependency injector
            dependencyObject = DependencyFetcher.GetDependency(dependencyType);
        }

        public override object CreateInstance(object parent)
        {
            throw new NotImplementedException();
        }

    }
}
