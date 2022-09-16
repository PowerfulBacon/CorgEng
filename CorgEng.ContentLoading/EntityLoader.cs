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
    public class EntityLoader
    {

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// Store a dictionary of types during parsing
        /// </summary>
        public static Dictionary<string, Type> TypePaths = null;

        /// <summary>
        /// List of all loaded definitions
        /// </summary>
        internal static Dictionary<string, DefinitionNode> LoadedDefinitions = new Dictionary<string, DefinitionNode>();

        public static IEnumerable<string> LoadedDefinitionNames
        {
            get => LoadedDefinitions.Keys;
        }

        /// <summary>
        /// The load queue
        /// </summary>
        private static Dictionary<string, XmlNode> XmlLoadQueue = new Dictionary<string, XmlNode>();

        [ModuleLoad]
        public static void LoadEntities()
        {
            //Generate a list of typepaths
            TypePaths = CorgEngMain.LoadedAssemblyModules
                .SelectMany(x => x.GetTypes())
                .OrderBy(x => x.FullName)
                .DistinctBy(x => x.Name)
                .ToDictionary(x => x.Name, x => x);

            //Clear existing nodes
            EntityCreator.EntityNodesByName.Clear();
            LoadedDefinitions.Clear();

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
                foreach (XmlNode parent in xmlDocument.ChildNodes)
                {
                    foreach (XmlNode node in parent.ChildNodes)
                    {
                        //Ignore declarations or comments
                        if (node is XmlDeclaration || node is XmlComment)
                            continue;
                        //Get the node name
                        string nodeName = node.Attributes["id"]?.Value;
                        //Make sure the node hasn't been preloaded already
                        if (string.IsNullOrEmpty(nodeName))
                        {
                            Logger.WriteLine($"Top level definition node {node.Name} in file {node.BaseURI} has no 'ID' attribute; skipping loading.", LogType.WARNING);
                            continue;
                        }
                        if (XmlLoadQueue.ContainsKey(nodeName))
                        {
                            Logger.WriteLine($"There exists 2 definition nodes with the ID '{nodeName}'. Only one of them will be loaded which may cause inconsistent or broken behaviour.", LogType.WARNING);
                            continue;
                        }
                        XmlLoadQueue.Add(nodeName, node);
                    }
                }
            }
            //Perform loading
            while (XmlLoadQueue.Count > 0)
            {
                //Take one
                string nodeName = XmlLoadQueue.Keys.First();
                XmlNode node = XmlLoadQueue[nodeName];
                //Remove the node from the processing queue
                XmlLoadQueue.Remove(nodeName);
                try
                {
                    //Load the node
                    RecursivelyParse(node);
                }
                catch (Exception e)
                {
                    //Throw a more detailed exception
                    throw new ContentLoadException($"An exception occured while loading {nodeName} in {node.BaseURI}.", e);
                }
            }
            //No longer required
            TypePaths = null;
        }

        /// <summary>
        /// Get the definition of something and load it if it isn't loaded
        /// </summary>
        /// <param name="definitionName"></param>
        /// <returns></returns>
        internal static DefinitionNode GetDefinition(string definitionName)
        {
            if (LoadedDefinitions.ContainsKey(definitionName))
            {
                return LoadedDefinitions[definitionName];
            }
            if (XmlLoadQueue.ContainsKey(definitionName))
            {
                //Load and return
                XmlNode node = XmlLoadQueue[definitionName];
                //Remove the node from the processing queue
                XmlLoadQueue.Remove(definitionName);
                //Load the node
                RecursivelyParse(node);
                //Return it
                return LoadedDefinitions[definitionName];
            }
            throw new ContentLoadException($"Could not locate the definition named '{definitionName}'.");
        }

        private static void RecursivelyParse(XmlNode node, DefinitionNode parentNode = null)
        {
            if (node is XmlText || node is XmlComment)
                return;
            //Create the node
            DefinitionNode createdNode;
            switch (node.Name.ToLower())
            {
                case "entities":
                    createdNode = null;
                    break;
                case "parameter":
                    createdNode = new ParameterNode(parentNode);
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
                case "array":
                    createdNode = new ArrayNode(parentNode);
                    break;
                case "element":
                    createdNode = new ElementNode(parentNode);
                    break;
                case "key":
                    createdNode = new KeyNode(parentNode);
                    break;
                case "value":
                    createdNode = new ValueNode(parentNode);
                    break;
                case "dictionary":
                    createdNode = new DictionaryNode(parentNode);
                    break;
                default:
                    Logger?.WriteLine($"Unknown node in entitiy definition file: {node.Name}.", LogType.ERROR);
                    return;
            }
            //Parse our own node
            createdNode?.ParseSelf(node);
            //Definition was loaded
            if (!string.IsNullOrEmpty(createdNode?.ID))
            {
                LoadedDefinitions.Add(createdNode.ID, createdNode);
            }
            //Parse children
            foreach (XmlNode child in node.ChildNodes)
            {
                RecursivelyParse(child, createdNode);
            }
        }

    }
}
