using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.ContentLoading.XmlDataStructures
{
    public class ListDef : PropertyDef
    {

        public new List<PropertyDef> Children { get; } = new List<PropertyDef>();

        public ListDef(string name) : base(name)
        { }

        public override void AddChild(PropertyDef property)
        {
            Children.Add(property);
            if (property.Tags.ContainsKey("Override"))
                throw new XmlException("The override tag cannot be used on lists!");
        }

        public override object GetValue(IVector<float> initializePosition)
        {
            List<object> values = new List<object>();
            foreach (PropertyDef property in GetChildren())
            {
                values.Add(property.GetValue(initializePosition));
            }
            return values;
        }

        public override IReadOnlyCollection<PropertyDef> GetChildren()
        {
            return Children;
        }

        /// <summary>
        /// Merge lists by getting elements from all
        /// </summary>
        protected override void UpdateFrom(PropertyDef overrider)
        {
            //Update all incoming properties
            foreach (PropertyDef incomingProperty in overrider.GetChildren())
            {
                AddChild(incomingProperty);
            }
        }

        public override PropertyDef Copy()
        {
            ListDef copy = new ListDef(Name);
            foreach (string key in Tags.Keys)
                copy.Tags.Add(key, Tags[key]);
            foreach (PropertyDef child in Children)
                copy.Children.Add(child.Copy());
            return copy;
        }
    }
}
