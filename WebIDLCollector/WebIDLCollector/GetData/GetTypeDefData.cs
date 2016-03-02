using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.GetData
{
    public partial class DataCollectors
    {
        private static readonly Regex TypeDefParser = new Regex(@"\s*typedef\s+(?<type>(\([^)]+\)|.+?))\s+(?<item>[^;]+);?", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static IEnumerable<TypeDefType> GetAllTypeDefs(string cleanString, SpecData specificationData)
        {
            var typeDefs = new List<TypeDefType>();

            foreach (var typeDefDefinition in from Match typeDefMatch in TypeDefParser.Matches(cleanString)
                                              select new TypeDefType
                                              {
                                                  Name = typeDefMatch.Groups["item"].Value.Trim(),
                                                  Type = typeDefMatch.Groups["type"].Value.Trim(),
                                                  SpecNames = new[] { specificationData.Name }
                                              })
            {
                if (!typeDefs.Contains(typeDefDefinition))
                {
                    typeDefs.Add(typeDefDefinition);
                }
                else
                {
                    Console.WriteLine("Duplicate typedef: " + typeDefDefinition.Name);
                }
            }

            return typeDefs;
        }
    }
}