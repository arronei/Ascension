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

        private static readonly Regex TypeCleaner = new Regex(@"\s+\?", RegexOptions.Compiled);

        private static readonly Regex OldTypeCleaner = new Regex(@"[a-z]*::", RegexOptions.Compiled);

        private static readonly Regex ParenCleaner = new Regex(@"\(\)", RegexOptions.Compiled);

        private static string CleanString(string value)
        {
            value = value.Trim().Trim('.').Trim();
            value = Regex.Replace(value, @"\s *//.*$", string.Empty, RegexOptions.Multiline);
            value = Regex.Replace(value, @"\s*(set)?raises\([^)]*?\)\s*;", ";");
            value = Regex.Replace(value, @"\s*(?<start>(\(|,)\s*)in\s+", "${start}");
            value = Regex.Replace(value, @"\s*", " ", RegexOptions.Singleline | RegexOptions.Multiline);
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
                        Type = OldTypeCleaner.Replace(TypeCleaner.Replace(m.Groups["type"].Value, "?"), string.Empty).Trim(),
                        ExtendedAttribute = m.Groups["extended"].Value.Trim(),
                        In = !string.IsNullOrWhiteSpace(m.Groups["in"].Value),
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