using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.GetData
{
    public static partial class DataCollectors
    {
        private static readonly Regex EnumParser = new Regex(@"enum\s+(?<name>[A-Z_a-z][0-9A-Z_a-z]*)\s*{\s*(?<enumvalues>[^}]+)\s*};?", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static IEnumerable<EnumType> GetAllEnums(string cleanString, SpecData specificationData)
        {
            var enums = new List<EnumType>();

            foreach (Match eTypeMatch in EnumParser.Matches(cleanString))
            {
                var enumDefinition = new EnumType
                {
                    Name = eTypeMatch.Groups["name"].Value,
                    SpecNames = new[] { specificationData.Name }
                };
                if (eTypeMatch.Groups["enumvalues"].Length > 0)
                {
                    enumDefinition.EnumValues = GetAllEnumValues(eTypeMatch.Groups["enumvalues"].Value);
                }

                if (!enums.Contains(enumDefinition))
                {
                    enums.Add(enumDefinition);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Duplicate enum: " + enumDefinition.Name);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return enums;
        }

        private static IEnumerable<string> GetAllEnumValues(string valueItems)
        {
            return valueItems.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim().Trim('"')).ToList();
        }
    }
}