using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CorgEng.ContentLoading.XmlDataStructures
{
    public class EntityDef : PropertyDef, IEntityDef
    {

        [UsingDependency]
        private static ILogger Log;

        public EntityDef(string name) : base(name)
        {
        }

        public static EntityDef ConvertToEntity(PropertyDef property)
        {
            EntityDef createdEntity = new EntityDef(property.Name);
            foreach (string key in property.Tags.Keys)
                createdEntity.Tags.Add(key, property.Tags[key]);
            foreach (string key in property.Children.Keys)
                createdEntity.Children.Add(key, property.Children[key].Copy());
            return createdEntity;
        }

        public override object GetValue(IVector<float> initializePosition)
        {
            return GetValue(initializePosition, false);
        }

        public object GetValue(IVector<float> initializePosition, bool useNameInsteadOfClassTag)
        {
            if (Tags.ContainsKey("Abstract"))
                throw new Exception("Cannot instantiate an abstract class");
            if (!useNameInsteadOfClassTag && !Tags.ContainsKey("Class"))
                throw new XmlException($"Property with name {Name} did not have the required tag Class.");
            //Get the class to instantiate
            string className = useNameInsteadOfClassTag ? Name : Tags["Class"];
            //Locate the type to load
            if (!EntityConfig.ClassTypeCache.ContainsKey(className))
                throw new XmlException($"Property with name {Name} has an invalid Class tag ({className} is not a known class, it must extend IInsantiable)");
            Type loadedType = EntityConfig.ClassTypeCache[className];
            //Instantiate the located type
            //Locate the first constructor
            ConstructorInfo firstConstructor = loadedType.GetConstructors()[0];
            //Invoke the first constructor located, either with the provided constructor parameters or nothing
            IInstantiatable created = firstConstructor.Invoke(new object[0]) as IInstantiatable;
            //The created class is not an instantiatable type
            if (created == null)
            {
                throw new XmlException($"The class {className} is not an instantiatable type. It must implement IInstantiable.");
            }
            //Set the typeDef
            created.TypeDef = this;
            //Call the pre init behaviour
            created.PreInitialize(initializePosition);
            //Set the properties of this object
            foreach (IPropertyDef property in GetChildren())
            {
                try
                {
                    if(!created.SetProperty(property.Name, property))
                        throw new NotImplementedException($"Unknown property name {property.Name}. Check configuration file for {Name}");
                }
                catch (Exception e)
                {
                    Log.WriteLine(e, LogType.ERROR);
                }
            }
            //Initialize the created thing
            created.Initialize(initializePosition);
            //Return the created thing
            return created;
        }

        public object Instantiate()
        {
            return GetValue(Vector<float>.Zero);
        }

        /// <summary>
        /// Instantiate the provided entity at the given position
        /// </summary>
        public object InstantiateAt(IVector<float> position)
        {
            return GetValue(position);
        }

        public override IPropertyDef Copy()
        {
            PropertyDef copy = new EntityDef(Name);
            foreach (string key in Tags.Keys)
                copy.Tags.Add(key, Tags[key]);
            foreach (string key in Children.Keys)
                copy.Children.Add(key, Children[key].Copy());
            return copy;
        }
    }
}
