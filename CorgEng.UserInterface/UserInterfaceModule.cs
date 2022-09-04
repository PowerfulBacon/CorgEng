using CorgEng.Core;
using CorgEng.Core.Modules;
using CorgEng.UserInterface.Attributes;
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

        [ModuleLoad]
        public static void OnModuleLoad()
        {
            KeyMethods = CorgEngMain.LoadedAssemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.GetCustomAttribute<UserInterfaceCodeCallbackAttribute>() != null)))
                .ToDictionary(key => key.GetCustomAttribute<UserInterfaceCodeCallbackAttribute>().KeyName, val => val);
        }

    }
}
