using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.DependencyInjection.Dependencies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CorgEng.DependencyInjection.Injection
{
    public static class DependencyInjector
    {

        /// <summary>
        /// Dictionary of all registered depenencies (created as singletons)
        /// </summary>
        private static Dictionary<Type, DependencyList> DependencyList = new Dictionary<Type, DependencyList>();

        public static bool InjectionCompleted { get; private set; } = false;

        /// <summary>
        /// Load the dependencies, called during module loading.
        /// </summary>
        [ModuleLoad(priority = true)]
        public static void LoadDependencyInjection()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //Go through all classes
            //Find those with the DependencyAttribute
            IEnumerable<Type> locatedDependencyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes().Where(exportedType =>
                {
                    return exportedType.IsClass && exportedType.GetCustomAttribute<DependencyAttribute>() != null;
                }));
            Console.WriteLine($"Located {locatedDependencyTypes.Count()} dependency types.");
            //Load those classes into the dependency list
            foreach (Type dependencyType in locatedDependencyTypes)
            {
                //Get the interface types
                Type[] interfaces = dependencyType.GetInterfaces();
                //Check if the dependency is default
                DependencyAttribute dependencyAttribute = dependencyType.GetCustomAttribute<DependencyAttribute>();
                //Load the dependency into the dependency list
                foreach (Type targetInterface in interfaces)
                {
                    DependencyList targetDependencyList;
                    //Locate the dependency list to add to
                    if (!DependencyList.ContainsKey(targetInterface))
                    {
                        targetDependencyList = new DependencyList();
                        DependencyList.Add(targetInterface, targetDependencyList);
                    }
                    else
                    {
                        targetDependencyList = DependencyList[targetInterface];
                    }
                    //Insert into the dependency list
                    //Check priority
                    if (targetDependencyList.ImplementedDependency == null || targetDependencyList.CurrentPriority < dependencyAttribute.priority)
                    {
                        //Instantiate the type
                        object instantiatedDependency = Activator.CreateInstance(dependencyType);
                        targetDependencyList.CurrentPriority = dependencyAttribute.priority;
                        targetDependencyList.ImplementedDependency = instantiatedDependency;
                    }
                }
            }
            Console.WriteLine($"Loaded {DependencyList.Count} dependency modules.");
            //Perform dependency injection
            InjectDependencies();
            stopwatch.Stop();
            Console.WriteLine($"Completed dependency loading and injection in {stopwatch.ElapsedMilliseconds}ms.");
            InjectionCompleted = true;
        }

        /// <summary>
        /// Locate all using dependency static attributes and inject the dependencies into them.
        /// </summary>
        private static void InjectDependencies()
        {
            //Locate all members we need to inject dependencies into
            IEnumerable<MemberInfo> requiredInjections = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes().SelectMany(exportedType =>
                {
                    return exportedType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Union<MemberInfo>(exportedType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                        .Where(memberInfo => memberInfo.GetCustomAttribute<UsingDependencyAttribute>() != null);
                }));
            //Perform injection
            foreach (MemberInfo memberInfo in requiredInjections)
            {
                try
                {
                    if (memberInfo is PropertyInfo property)
                    {
                        //Locate the type of the property
                        Type desiredType = property.PropertyType;
                        property.SetValue(null, GetDependencyInstance(desiredType));
                    }
                    else if (memberInfo is FieldInfo field)
                    {
                        Type desiredType = field.FieldType;
                        field.SetValue(null, GetDependencyInstance(desiredType));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Injection failure on {memberInfo.Name} at {memberInfo.DeclaringType}:");
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine($"Injected {requiredInjections.Count()} dependencies.");
        }

        private static object GetDependencyInstance(Type interfaceType)
        {
            //Locate dependency
            if (DependencyList.ContainsKey(interfaceType))
            {
                return DependencyList[interfaceType].ImplementedDependency;
            }
            //Dependency injection issue
            return null;
        }

    }
}
