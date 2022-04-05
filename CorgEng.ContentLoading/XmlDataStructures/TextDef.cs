using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.ContentLoading.XmlDataStructures
{
    public class TextDef : PropertyDef
    {

        private string value;

        public TextDef(string name, string value) : base(name)
        {
            this.value = value;
        }

        public override object GetValue(IVector<float> initializePosition)
        {
            return value;
        }

        public override void UpdateFrom(IPropertyDef overrider)
        {
            value = ((TextDef)overrider).value;
            base.UpdateFrom(overrider);
        }

        public override IPropertyDef Copy()
        {
            TextDef copy = new TextDef(Name, value);
            foreach (string key in Tags.Keys)
                copy.Tags.Add(key, Tags[key]);
            foreach (string key in Children.Keys)
                copy.Children.Add(key, Children[key].Copy());
            return copy;
        }
    }
}
