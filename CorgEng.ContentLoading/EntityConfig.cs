using CorgEng.ContentLoading.XmlDataStructures;
using CorgEng.Core;
using CorgEng.GenericInterfaces.ContentLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.ContentLoading
{
    public static class EntityConfig
    {

        /// <summary>
        /// Dictionary containing all loaded entity defs.
        /// </summary>
        public static Dictionary<string, EntityDef> LoadedEntityDefs = new Dictionary<string, EntityDef>();

        /// <summary>
        /// All of the loaded constant values
        /// </summary>
        public static Dictionary<string, PropertyDef> LoadedConstants = new Dictionary<string, PropertyDef>();

        /// <summary>
        /// A cache of all classes
        /// </summary>
        public static Dictionary<string, Type> ClassTypeCache = CorgEngMain.LoadedAssemblyModules
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IInstantiatable).IsAssignableFrom(type))
            .GroupBy(type => type.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(typeGroup => typeGroup.Key, typeGroup => typeGroup.Last(), StringComparer.OrdinalIgnoreCase);

    }
}
