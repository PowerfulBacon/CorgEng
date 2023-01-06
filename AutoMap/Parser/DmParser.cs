using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoMap.Parser
{
    internal class DmParser
    {

        private static Regex typepathRegex = new Regex(@"\/((?:atom|obj)(?:\/\w*)+)\n(.|\s)*?(?=([\n\r]+\/\w*)+|$)", RegexOptions.Compiled);
        private static Regex nameRegex = new Regex(@"name\s*=\s*""(.*?)""", RegexOptions.Compiled);
        private static Regex iconRegex = new Regex(@"icon\s*=\s*'(.*?)'", RegexOptions.Compiled);
        private static Regex iconStateRegex = new Regex(@"icon_state\s*=\s*""(.*?)""", RegexOptions.Compiled);

        public static ParsedEnvironment ParseFile(ParsedEnvironment environment, string fileDir)
        {
            string fileText = File.ReadAllText(fileDir);
            MatchCollection collection = typepathRegex.Matches(fileText);
            foreach (Match match in collection)
            {
                Match nameMatch = nameRegex.Match(match.Value);
                Match iconMatch = iconRegex.Match(match.Value);
                Match iconStateMatch = iconStateRegex.Match(match.Value);
                environment.AddType($"{match.Groups[1]}", nameMatch, iconMatch, iconStateMatch);
            }
            return environment;
        }

    }
}
