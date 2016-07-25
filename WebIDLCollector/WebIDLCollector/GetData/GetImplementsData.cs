using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.GetData
{
    public partial class DataCollectors
    {
        private static readonly Regex ImplementsParser = new Regex(@"\s*(?<destination>[A-Z_a-z][0-9A-Z_a-z]*)\s+implements\s+(?<originator>[A-Z_a-z][0-9A-Z_a-z]*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static IEnumerable<ImplementsType> GetAllImplements(string cleanString, SpecData specificationData)
        {
            var implements = new List<ImplementsType>();

            foreach (var implementsDefinition in from Match implementsMatch in ImplementsParser.Matches(cleanString)
                                                 select new ImplementsType(implementsMatch.Groups["destination"].Value.Trim(), implementsMatch.Groups["originator"].Value.Trim())
                                                 {
                                                     SpecNames = new[] { specificationData.Name }
                                                 })
            {
                if (!implements.Contains(implementsDefinition))
                {
                    implements.Add(implementsDefinition);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Duplicate implements: " + implementsDefinition.DestinationInterface + " " + implementsDefinition.OriginatorInterface);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return implements;
        }
    }
}