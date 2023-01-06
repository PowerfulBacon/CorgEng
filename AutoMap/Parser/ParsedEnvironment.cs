using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoMap.Parser
{
    public class ParsedEnvironment
    {

        TypePathTree topLevel = new TypePathTree();

        public void AddType(string typePath, Match name, Match icon, Match iconState)
        {
            string[] splitPath = typePath.Split('/');
            //Navigate to the location
            TypePathTree current = topLevel;
            foreach (string location in splitPath)
                current = current.NavigateTo(location);
            current.fullPath = typePath;
            current.name = name.Success ? name.Value : null;
            current.icon = icon.Success ? icon.Value : null;
            current.iconState = iconState.Success ? iconState.Value : null;
        }

        private class TypePathTree
        {

            public Dictionary<string, TypePathTree> Children = new Dictionary<string, TypePathTree>();

            public string fullPath;

            public string? name;
            public string? icon;
            public string? iconState;

            public TypePathTree NavigateTo(string path)
            {
                if (Children.TryGetValue(path, out var value))
                    return value;
                TypePathTree newPath = new TypePathTree();
                Children.Add(path, newPath);
                return newPath;
            }

        }

    }
}
