using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.ContentLoading.XmlDataStructures
{
    public class PropertyDef : IPropertyDef
    {

        private static int num = 0;

        public int UUID { get; } = num++;

        public string Name { get; }

        /// <summary>
        /// List of tags inside the XML (Class)
        /// </summary>
        public Dictionary<string, string> Tags { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Children components
        /// </summary>
        public Dictionary<string, IPropertyDef> Children { get; } = new Dictionary<string, IPropertyDef>();

        public virtual IPropertyDef Copy()
        {
            IPropertyDef copy = new PropertyDef(Name);
            foreach (string key in Tags.Keys)
                copy.Tags.Add(key, Tags[key]);
            foreach (string key in Children.Keys)
                copy.Children.Add(key, Children[key].Copy());
            return copy;
        }

        /// <summary>
        /// Return the instantiated value of this property.
        /// Takes in constructor parameters for anything with children taking in ctor args.
        /// (Usually Vector<float> position)
        /// </summary>
        public virtual object GetValue(IVector<float> initializePosition)
        {
            //Return ourselves for special handling
            return this;
        }

        public PropertyDef(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Add a child element
        /// </summary>
        public virtual void AddChild(IPropertyDef property)
        {
            //Update existing
            if (Children.ContainsKey(property.Name))
            {
                if (property.Tags.ContainsKey("Override"))
                    Children.Remove(property.Name);
                else
                {
                    Children[property.Name].UpdateFrom(property);
                    return;
                }
            }
            Children.Add(property.Name, property);
        }

        /// <summary>
        /// Take an incoming property and override existing.
        /// Example:
        /// <Entity Name="Parent">
        ///   <List>
        ///     <ListEntry>5</ListEntry>
        ///     <ListEntry>8</ListEntry>
        ///   </List>
        ///   <OverrideMe>100</OverrideMe>
        /// </Entity>
        /// <Entity Name="Child" ParentName="Parent">
        ///   <OverrideMe>200</OverrideMe>
        /// </Entity>
        /// </summary>
        public virtual void UpdateFrom(IPropertyDef overrider)
        {
            //Update all incoming properties
            foreach (IPropertyDef incomingProperty in overrider.GetChildren())
            {
                //We already have this property, update it
                if (Children.ContainsKey(incomingProperty.Name))
                {
                    Children[incomingProperty.Name].UpdateFrom(incomingProperty);
                }
                else
                {
                    AddChild(incomingProperty);
                }
            }
        }

        public virtual IReadOnlyCollection<IPropertyDef> GetChildren()
        {
            return Children.Values;
        }

        /// <summary>
        /// Adds a tag, overriding existing tags
        /// </summary>
        public void AddTag(string name, string value)
        {
            if (Tags.ContainsKey(name))
                Tags[name] = value;
            else
                Tags.Add(name, value);
        }

        /// <summary>
        /// Check if our dependancies have been loaded.
        /// If they haven't loading of this item needs to be moved to the back of the queue.
        /// </summary>
        public bool DependanciesLoaded()
        {
            if (Tags.ContainsKey("ParentName"))
            {
                //Our parent has been loaded
                if (EntityConfig.LoadedEntityDefs.ContainsKey(Tags["ParentName"]))
                    return true;
                //Our parent hasn't been loaded
                return false;
            }
            return true;
        }

        public void InheritFromDependancy()
        {
            if (Tags.ContainsKey("ParentName"))
                InheritFrom(Tags["ParentName"]);
        }

        /// <summary>
        /// Get the parent of this object and inherit it's properties.
        /// Will also inherit parent's class tag.
        /// </summary>
        public void InheritFrom(string name)
        {
            //Load the parent
            EntityDef parent = EntityConfig.LoadedEntityDefs[name];
            //Inherit parent's class tag
            if (parent.Tags.ContainsKey("Class") && !Tags.ContainsKey("Class"))
                Tags.Add("Class", parent.Tags["Class"]);
            //Inherit parent's properties
            foreach (string superPropertyName in parent.Children.Keys)
            {
                //Inherit the child
                Children.Add(superPropertyName, parent.Children[superPropertyName].Copy());
            }
        }

        /// <summary>
        /// Get a child by name
        /// </summary>
        public IPropertyDef GetChild(string name)
        {
            if (Children.ContainsKey(name))
                return Children[name];
            return null;
        }

    }
}
