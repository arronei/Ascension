using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.GetData
{
    public static partial class DataCollectors
    {
        private static readonly Regex GroupingCleaner = new Regex(@"[\(\)\s]+", RegexOptions.Compiled);

        private static string CleanString(string value)
        {
            value = value.Trim().Trim('.').Trim();
            value = Regex.Replace(value, @"\s*(set)?raises\([^)]*?\)\s*;", ";");
            value = Regex.Replace(value, @"(?<start>(\(|,)\s*)in\s+", "${start}");
            return value.Trim();
        }

        private static IEnumerable<Argument> GetArgTypes(string args)
        {
            var arguments = args.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var argumentList = new List<Argument>();

            foreach (var argument in arguments.Select(argument => argument.Trim()))
            {
                if (ArgumentParser.IsMatch(argument))
                {
                    var m = ArgumentParser.Match(argument);

                    var argumentItem = new Argument(m.Groups["name"].Value.Trim())
                    {
                        Type = Regex.Replace(Regex.Replace(m.Groups["type"].Value, @"\s+\?", "?"), @"[a-z]*::", string.Empty).Trim(),
                        ExtendedAttribute = m.Groups["extended"].Value.Trim(),
                        //In = !string.IsNullOrWhiteSpace(m.Groups["in"].Value),
                        Optional = !string.IsNullOrWhiteSpace(m.Groups["optional"].Value),
                        Ellipsis = !string.IsNullOrWhiteSpace(m.Groups["ellipsis"].Value),
                        Value = m.Groups["value"].Value.Trim()
                    };

                    if (!string.IsNullOrWhiteSpace(argumentItem.ExtendedAttribute))
                    {
                        foreach (Match aep in AttributeExtendedParser.Matches(argumentItem.ExtendedAttribute))
                        {
                            argumentItem.Clamp = !string.IsNullOrWhiteSpace(aep.Groups["clamp"].Value.Trim());
                            argumentItem.EnforceRange = !string.IsNullOrWhiteSpace(aep.Groups["enforcerange"].Value.Trim());
                            argumentItem.TreatNullAs = aep.Groups["treatnullas"].Value.Trim();
                            argumentItem.TreatUndefinedAs = aep.Groups["treatundefinedas"].Value.Trim();
                        }
                    }

                    argumentList.Add(argumentItem);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid argument- " + argument);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    throw new ArgumentException();
                }
            }

            return argumentList;
        }
    }
}