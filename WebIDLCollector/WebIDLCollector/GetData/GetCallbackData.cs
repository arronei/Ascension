using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.GetData
{
    public partial class DataCollectors
    {
        private static readonly Regex CallbackParser = new Regex(@"(\[(?<extended>[^\]]+)\]\s*)?callback\s+(?<name>\w+)\s*=\s*(?<type>.+?)\s*\((?<args>[^\)]+)?\);", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex CallbackExtendedParser = new Regex(@"(?<extended>
        ((?<treatnonobjectasnull>treatnonobjectasnull))
        (\s*,\s*
        ((?<treatnonobjectasnull>treatnonobjectasnull))
        )*
        )", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        public static IEnumerable<CallbackType> GetAllCallbacks(string cleanString, SpecData specificationData)
        {
            var callbackDefs = new List<CallbackType>();

            foreach (var callbackDefinition in from Match callbackMatch in CallbackParser.Matches(cleanString)
                                               select new CallbackType(callbackMatch.Groups["name"].Value.Trim())
                                               {
                                                   Type = callbackMatch.Groups["type"].Value.Trim(),
                                                   Args = callbackMatch.Groups["args"].Value.Trim(),
                                                   ExtendedAttribute = callbackMatch.Groups["extended"].Value.Trim(),
                                                   SpecNames = new[] { specificationData.Name }
                                               })
            {
                if (!string.IsNullOrWhiteSpace(callbackDefinition.ExtendedAttribute))
                {
                    //Designed for future expansion of extended attributes
                    foreach (Match cep in CallbackExtendedParser.Matches(callbackDefinition.ExtendedAttribute))
                    {
                        callbackDefinition.TreatNonObjectAsNull = !string.IsNullOrWhiteSpace(cep.Groups["treatnonobjectasnull"].Value.Trim());
                    }
                }

                try
                {
                    callbackDefinition.ArgTypes = GetArgTypes(callbackDefinition.Args);
                }
                catch (ArgumentException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fail argument for callback- " + callbackDefinition.Name);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    continue;
                }

                if (!callbackDefs.Contains(callbackDefinition))
                {
                    callbackDefs.Add(callbackDefinition);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Duplicate callback: " + callbackDefinition.Name);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return callbackDefs;
        }
    }
}