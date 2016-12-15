using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebIDLCollector.IDLTypes;
using WebIDLCollector.Utilities;

namespace WebIDLCollector.GetData
{
    public static partial class DataCollectors
    {
        private static readonly Regex ArgumentParser = new Regex(@"^\s*(\[(?<extended>[^\]]+)]\s*)?(
        ((?<in>in)\s+)?
        ((?<optional>optional)\s+)?
        (?<type>[^\.=]+)
        (\s*(?<ellipsis>\.\.\.))?\s+
        ((?<name>[^=\s]+)
        (\s*=\s*(?<value>.+?))?))$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex ArgumentExtendedParser = new Regex(@"(?<clamp>clamp)(,|$)|
        (?<enforcerange>enforcerange)(,|$)|
        (treatnullas(\s*=\s*(?<treatnullas>[^\s,\]]+)))(,|$)|
        (treatundefinedas(\s*=\s*(?<treatundefinedas>[^\s,\]]+)))(,|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static string CleanString(string value)
        {
            value = value.Trim().Trim('.').Trim();
            value = Regex.Replace(value, @"\s *//.*$", string.Empty, RegexOptions.Multiline);
            value = Regex.Replace(value, @"\s*(set)?raises\([^)]*?\)\s*;", ";");
            value = Regex.Replace(value, @"\s*(?<start>(\(|,)\s*)in\s+", "${start}");
            value = RegexLibrary.WhitespaceCleaner.Replace(value, " ");
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
                        Type = RegexLibrary.OldTypeCleaner.Replace(RegexLibrary.TypeCleaner.Replace(m.Groups["type"].Value, "?"), string.Empty).Trim(),
                        ExtendedAttribute = m.Groups["extended"].Value.Trim(),
                        Optional = !string.IsNullOrWhiteSpace(m.Groups["optional"].Value),
                        Ellipsis = !string.IsNullOrWhiteSpace(m.Groups["ellipsis"].Value),
                        Value = m.Groups["value"].Value.Trim()
                    };

                    if (!string.IsNullOrWhiteSpace(argumentItem.ExtendedAttribute))
                    {
                        foreach (Match aep in ArgumentExtendedParser.Matches(argumentItem.ExtendedAttribute))
                        {
                            argumentItem.Clamp = !string.IsNullOrWhiteSpace(aep.Groups["clamp"].Value.Trim());
                            argumentItem.EnforceRange = !string.IsNullOrWhiteSpace(aep.Groups["enforcerange"].Value.Trim());
                            argumentItem.TreatNullAs = aep.Groups["treatnullas"].Value.Trim();
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