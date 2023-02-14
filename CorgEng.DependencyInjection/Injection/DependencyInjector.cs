using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Logging;
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
            //Find those with the DependencyAttribute
            IEnumerable<Type> locatedDependencyTypes = CorgEngMain.LoadedAssemblyModules.SelectMany(assembly => assembly.GetTypes().Where(
                exportedType => exportedType.IsClass && exportedType.GetCustomAttribute<DependencyAttribute>() != null));
            Console.WriteLine($"Located {locatedDependencyTypes.Count()} dependency types.");
            //Load those classes into the dependency list
            foreach (Type dependencyType in locatedDependencyTypes)
            {
                //Get the interface types
                Type[] interfaces = dependencyType.GetInterfaces();
                //Locate the attribute and get the reflection object
                //MASSIVE REFLECTION HACK: THERES A BUG IN REFLECTION WITH LOADED ASSEMBLIES
                DependencyAttribute locatedAttribute = dependencyType.GetCustomAttribute<DependencyAttribute>();
                int priority = locatedAttribute.priority;
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
                    if (targetDependencyList.ImplementedDependency == null || targetDependencyList.CurrentPriority < priority)
                    {
                        //Instantiate the type
                        object instantiatedDependency = Activator.CreateInstance(dependencyType);
                        targetDependencyList.CurrentPriority = priority;
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
            IEnumerable<MemberInfo> requiredInjections = CorgEngMain.LoadedAssemblyModules.SelectMany(assembly => assembly.GetTypes().SelectMany(exportedType =>
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
                    // Perform injection
                    object injectedValue = null;
                    if (memberInfo is PropertyInfo property)
                    {
                        //Locate the type of the property
                        Type desiredType = property.PropertyType;
                        injectedValue = GetDependencyInstance(desiredType);
                        property.SetValue(null, injectedValue);
                    }
                    else if (memberInfo is FieldInfo field)
                    {
                        Type desiredType = field.FieldType;
                        injectedValue = GetDependencyInstance(desiredType);
                        field.SetValue(null, injectedValue);
                    }
                    if (injectedValue == null)
                    {
                        Console.WriteLine($"Injection failure on {memberInfo.Name} at {memberInfo.DeclaringType}: Couldn't locate dependency.");
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

        internal static object GetDependencyInstance(Type interfaceType)
        {
            //Locate dependency
            if (DependencyList.ContainsKey(interfaceType))
            {
                return DependencyList[interfaceType].ImplementedDependency;
            }
            //Dependency injection issue
            return null;
        }

        /// <summary>
        /// Overrides a specified dependency.
        /// Incredibly slow due to using reflection.
        /// </summary>
        /// <typeparam name="DependencyInterface"></typeparam>
        /// <param name="instantiatedDependency"></param>
        public static void OverrideDependency<DependencyInterface>(DependencyInterface instantiatedDependency)
        {
            //Locate everything we need to replace
            //Locate all members we need to inject dependencies into
            IEnumerable<MemberInfo> requiredInjections = CorgEngMain.LoadedAssemblyModules.SelectMany(assembly => assembly.GetTypes().SelectMany(exportedType =>
            {
                return exportedType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(fieldInfo => fieldInfo.FieldType == typeof(DependencyInterface)
                        && fieldInfo.GetCustomAttribute<UsingDependencyAttribute>() != null)
                    .Union<MemberInfo>(exportedType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(propertyInfo => propertyInfo.PropertyType == typeof(DependencyInterface)
                            && propertyInfo.GetCustomAttribute<UsingDependencyAttribute>() != null));
            }));

            //Perform injection
            foreach (MemberInfo memberInfo in requiredInjections)
            {
                try
                {
                    if (instantiatedDependency == null)
                    {
                        Console.WriteLine($"Injection failure on {memberInfo.Name} at {memberInfo.DeclaringType}: Could not locate dependency type.");
                    }
                    if (memberInfo is PropertyInfo property)
                    {
                        //Locate the type of the property
                        Type desiredType = property.PropertyType;
                        property.SetValue(null, instantiatedDependency);
                    }
                    else if (memberInfo is FieldInfo field)
                    {
                        Type desiredType = field.FieldType;
                        field.SetValue(null, instantiatedDependency);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Injection failure on {memberInfo.Name} at {memberInfo.DeclaringType}:");
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine($"Injected override dependency ({typeof(DependencyInterface)}) on {requiredInjections.Count()} instances");
        }

    }
}
