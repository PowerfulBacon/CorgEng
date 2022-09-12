using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.UserInterface.Attributes;
using CorgEng.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface
{
    public static class UserInterfaceModule
    {

        internal static Dictionary<string, MethodInfo> KeyMethods = new Dictionary<string, MethodInfo>();

        [UsingDependency]
        private static ILogger Logger;

        [ModuleLoad]
        public static void OnModuleLoad()
        {
            //Get key methods
            KeyMethods = CorgEngMain.LoadedAssemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.GetCustomAttribute<UserInterfaceCodeCallbackAttribute>() != null)))
                .ToDictionary(key => key.GetCustomAttribute<UserInterfaceCodeCallbackAttribute>().KeyName, val => val);

            //Validate
            foreach (MethodInfo method in KeyMethods.Values)
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(IUserInterfaceComponent))
                {
                    Logger.WriteLine($"The method {method.Name} in {method.DeclaringType.Name} has invalid parameters. It must have a single parameter of type UserInterfaceComponent.", LogType.ERROR);
                }
            }
        }

    }
}
