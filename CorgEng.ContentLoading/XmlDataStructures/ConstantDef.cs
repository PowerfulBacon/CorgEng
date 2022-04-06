using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.ContentLoading.XmlDataStructures
{
    public class ConstantDef : PropertyDef
    {

        public ConstantDef(string name) : base(name)
        { }

        public override object GetValue(IVector<float> initializePosition)
        {
            return EntityConfig.LoadedConstants[GetChild("Constant").Tags["Name"]].GetValue(initializePosition);
        }

        public override IPropertyDef Copy()
        {
            IPropertyDef copy = new ConstantDef(Name);
            foreach (string key in Tags.Keys)
                copy.Tags.Add(key, Tags[key]);
            foreach (string key in Children.Keys)
                copy.Children.Add(key, Children[key].Copy());
            return copy;
        }
    }
}
