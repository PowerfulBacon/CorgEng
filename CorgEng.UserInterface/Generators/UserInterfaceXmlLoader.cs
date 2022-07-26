using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UserInterface.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CorgEng.UserInterface.Generators
{
    [Dependency]
    public class UserInterfaceXmlLoader : IUserInterfaceXmlLoader
    {

        [UsingDependency]
        private static IUserInterfaceComponentFactory UserInterfaceComponentFactory;

        [UsingDependency]
        private static IAnchorFactory AnchorFactory;

        [UsingDependency]
        private static IAnchorDetailFactory AnchorDetailFactory;

        [UsingDependency]
        private static ILogger Logger;

        public IUserInterfaceComponent LoadUserInterface(string filepath)
        {
            try
            {
                //Load the XML file provided
                XElement userInterfaceElement = XElement.Load(filepath);
                //Load the interface component
                return LoadFromXmlComponent(userInterfaceElement);
            }
            catch (Exception e)
            {
                Logger?.WriteLine(e, LogType.ERROR);
                return null;
            }
        }

        private IUserInterfaceComponent LoadFromXmlComponent(XElement node, IUserInterfaceComponent parent = null)
        {
            //Load specific information from the config
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters = node.Attributes().ToDictionary(attribute => attribute.Name.LocalName, attribute => attribute.Value);
            //Create a generic user interface component
            IUserInterfaceComponent currentElement = UserInterfaceComponentFactory.CreateUserInterfaceComponent(
                node.Name.LocalName,
                parent,
                AnchorFactory.CreateAnchor(
                    GetAnchorDetails(node, AnchorDirections.LEFT),
                    GetAnchorDetails(node, AnchorDirections.RIGHT),
                    GetAnchorDetails(node, AnchorDirections.TOP),
                    GetAnchorDetails(node, AnchorDirections.BOTTOM)
                ),
                parameters
                );
            //Parse children
            foreach (XElement childElement in node.Elements())
            {
                //Children are automatically added when created with a parent
                LoadFromXmlComponent(childElement, currentElement);
            }
            //Return the current one
            return currentElement;
        }

        private IAnchorDetails GetAnchorDetails(XElement node, AnchorDirections side)
        {
            XAttribute attribute = null;
            //Fetch the correct attribute
            switch (side)
            {
                case AnchorDirections.BOTTOM:
                    attribute = node.Attribute(XName.Get("bottomAnchor"));
                    break;
                case AnchorDirections.TOP:
                    attribute = node.Attribute(XName.Get("topAnchor"));
                    break;
                case AnchorDirections.LEFT:
                    attribute = node.Attribute(XName.Get("leftAnchor"));
                    break;
                case AnchorDirections.RIGHT:
                    attribute = node.Attribute(XName.Get("rightAnchor"));
                    break;
            }
            //Create a default attribute
            if (attribute == null)
                return AnchorDetailFactory.CreateAnchorDetails(side, AnchorUnits.PIXELS, 0, true);
            //Create our anchor
            string attributeValue = attribute.Value;
            //Parse the attribute value
            string[] splitAttribute = attributeValue.Split(':');
            //Determine the side name
            AnchorDirections anchorDirection = side;
            switch (splitAttribute[0])
            {
                case "left":
                    anchorDirection = AnchorDirections.LEFT;
                    break;
                case "right":
                    anchorDirection = AnchorDirections.RIGHT;
                    break;
                case "top":
                    anchorDirection = AnchorDirections.TOP;
                    break;
                case "bottom":
                    anchorDirection = AnchorDirections.BOTTOM;
                    break;
            }
            //Determine the value
            double valueAmount = 0;
            double.TryParse(splitAttribute[1].Replace("%", "").Replace("!", ""), out valueAmount);
            //Determine strictness
            bool strict = splitAttribute[1].Contains("!");
            //Determine units
            AnchorUnits anchorUnits = splitAttribute[1].Contains("%") ? AnchorUnits.PERCENTAGE : AnchorUnits.PIXELS;
            return AnchorDetailFactory.CreateAnchorDetails(anchorDirection, anchorUnits, valueAmount, strict);
        }

    }
}
