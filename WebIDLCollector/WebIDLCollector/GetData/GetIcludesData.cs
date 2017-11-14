using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.GetData
{
    public static partial class DataCollectors
    {
        private static readonly Regex IncludesParser = new Regex(@"\s*(?<destination>[A-Z_a-z][0-9A-Z_a-z]*)\s+includes\s+(?<originator>[A-Z_a-z][0-9A-Z_a-z]*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static IEnumerable<ImplementsType> GetAllIncludes(string cleanString, SpecData specificationData)
        {
            var includes = new List<ImplementsType>();

            foreach (var includesDefinition in from Match includesMatch in IncludesParser.Matches(cleanString)
                                                 select new ImplementsType(includesMatch.Groups["destination"].Value.Trim(), includesMatch.Groups["originator"].Value.Trim())
                                                 {
                                                     SpecNames = new[] { specificationData.Name }
                                                 })
            {
                if (!includes.Contains(includesDefinition))
                {
                    includes.Add(includesDefinition);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Duplicate implements: " + includesDefinition.DestinationInterface + " " + includesDefinition.OriginatorInterface);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return includes;
        }
    }
}