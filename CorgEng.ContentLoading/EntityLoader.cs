using CorgEng.ContentLoading.DefinitionNodes;
using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.ContentLoading
{
    internal class EntityLoader
    {

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// Store a dictionary of types during parsing
        /// </summary>
        public static Dictionary<string, Type> TypePaths = null;

        [ModuleLoad]
        public static void LoadEntities()
        {
            //Generate a list of typepaths
            TypePaths = CorgEngMain.LoadedAssemblyModules
                .SelectMany(x => x.GetTypes())
                .OrderBy(x => x.FullName)
                .DistinctBy(x => x.Name)
                .ToDictionary(x => x.Name, x => x);

            //Load all files
            foreach (string fileName in Directory.GetFiles(".", "*.xml", SearchOption.AllDirectories))
            {
                //Load the XML document
                XmlDocument xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.Load(fileName);
                }
                catch (XmlException e)
                {
                    Logger?.WriteLine(e, LogType.ERROR);
                    continue;
                }
                //Check the root element
                if (xmlDocument.LastChild?.Name != "Entities")
                    continue;
                foreach (XmlNode node in xmlDocument.ChildNodes)
                {
                    if (node is XmlDeclaration || node is XmlComment)
                        continue;
                    RecursivelyParse(node);
                }
            }
            //No longer required
            TypePaths = null;
        }

        private static void RecursivelyParse(XmlNode node, DefinitionNode parentNode = null)
        {
            //Create the node
            DefinitionNode createdNode;
            switch (node.Name.ToLower())
            {
                case "entities":
                    createdNode = null;
                    break;
                case "entity":
                    createdNode = new EntityNode(parentNode);
                    break;
                case "property":
                    createdNode = new PropertyNode(parentNode);
                    break;
                case "object":
                    createdNode = new ObjectNode(parentNode);
                    break;
                case "component":
                    createdNode = new ComponentNode(parentNode);
                    break;
                case "dependency":
                    createdNode = new DependencyNode(parentNode);
                    break;
                case "instance":
                    createdNode = new InstanceNode(parentNode);
                    break;
                default:
                    Logger?.WriteLine($"Unknown node in entitiy definition file: {node.Name}.", LogType.ERROR);
                    return;
            }
            //Parse our own node
            createdNode?.ParseSelf(node);
            //Parse children
            foreach (XmlNode child in node.ChildNodes)
            {
                RecursivelyParse(child, createdNode);
            }
        }

    }
}
