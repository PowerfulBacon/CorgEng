using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.ContentLoading.XmlDataStructures
{
    public class BooleanDef : PropertyDef
    {

        private bool value;

        public BooleanDef(string name, string value) : base(name)
        {
            string trimmedValue = value.Trim();
            this.value = trimmedValue == "true" ? true : trimmedValue == "false" ? false : throw new XmlException($"Unable to parse boolean value of {value}.");
        }

        public override object GetValue(IVector<float> initializePosition)
        {
            return value;
        }

        protected override void UpdateFrom(PropertyDef overrider)
        {
            value = ((BooleanDef)overrider).value;
            base.UpdateFrom(overrider);
        }

        public override PropertyDef Copy()
        {
            BooleanDef copy = new BooleanDef(Name, value ? "true" : "false");
            foreach (string key in Tags.Keys)
                copy.Tags.Add(key, Tags[key]);
            foreach (string key in Children.Keys)
                copy.Children.Add(key, Children[key].Copy());
            return copy;
        }
    }
}
