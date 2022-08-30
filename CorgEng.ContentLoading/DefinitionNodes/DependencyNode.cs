using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.ContentLoading.DefinitionNodes;
using CorgEng.GenericInterfaces.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

        /// <summary>
        /// The method that we are calling
        /// </summary>
        private MethodInfo methodToCall;

        private string[] dynamicKeys;

        private bool[] isDynamic;

        private bool hasDynamicParams = false;

        private object[] parameters;

        public DependencyNode(DefinitionNode parent) : base(parent)
        {
        }

        public override void ParseSelf(XmlNode node)
        {
            //Perform base parsing actions
            base.ParseSelf(node);
            //Get the type name
            string typeName = node.Attributes["type"].Value;
            //The type
            Type dependencyType = EntityLoader.TypePaths[typeName];
            //Get the dependency object from the dependency injector
            dependencyObject = DependencyFetcher.GetDependency(dependencyType);
            if (dependencyObject == null)
            {
                throw new ContentLoadException($"Unable to find dependency of type {typeName}.");
            }
            //Get the method
            string methodName = node.Attributes["method"].Value;
            methodToCall = dependencyType.GetMethod(methodName);
            if (methodToCall == null)
            {
                throw new ContentLoadException($"Unable to find method of type {typeName} on {dependencyType}.");
            }
            //Get the parameters
            parameters = new object[node.ChildNodes.Count];
            isDynamic = new bool[node.ChildNodes.Count];
            dynamicKeys = new string[node.ChildNodes.Count];
            for (int i = 0; i < node.ChildNodes.Count; i ++)
            {
                //Parse the static value of the node
                if (!(node.ChildNodes[i].FirstChild is XmlElement))
                {
                    parameters[i] = TypeDescriptor.GetConverter(methodToCall.GetParameters()[i].ParameterType).ConvertFromString(node.ChildNodes[i].InnerText.Trim());
                }
                //Mark the node as dynamic
                else
                {
                    //TODO: Allow for dynamic object parameters
                    hasDynamicParams = true;
                    isDynamic[i] = true;
                    dynamicKeys[i] = node.FirstChild.InnerText.Trim();
                }
            }
        }

        public override object CreateInstance(object parent, Dictionary<string, object> instanceRefs)
        {
            //Set the parameters
            if (hasDynamicParams)
            {
                for (int i = 0; i < isDynamic.Length; i++)
                {
                    if (isDynamic[i])
                    {
                        parameters[i] = instanceRefs[dynamicKeys[i]];
                    }
                }
            }
            //Call the method and get the result
            object result = methodToCall.Invoke(dependencyObject, parameters);
            //Store the created object
            if (Key != null)
            {
                instanceRefs.Add(Key, result);
            }
            return result;
        }

    }
}
