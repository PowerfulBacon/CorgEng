using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.ContentLoading
{
    public interface IPropertyDef
    {
        Dictionary<string, IPropertyDef> Children { get; }
        string Name { get; }
        Dictionary<string, string> Tags { get; }
        int UUID { get; }

        void AddChild(IPropertyDef property);
        void AddTag(string name, string value);
        IPropertyDef Copy();
        bool DependanciesLoaded();
        IPropertyDef GetChild(string name);
        IReadOnlyCollection<IPropertyDef> GetChildren();
        object GetValue(IVector<float> initializePosition);
        void InheritFrom(string name);
        void InheritFromDependancy();
        void UpdateFrom(IPropertyDef overrider);
    }

}
