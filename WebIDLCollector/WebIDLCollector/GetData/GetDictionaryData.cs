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
        private static readonly Regex DictionaryParser = new Regex(@"(\[(?<extended>[^\]]+)\]\s*)?(?<partial>partial)?\s*dictionary\s+(?<name>\w+)(?:\s*:\s*(?<inherits>[\w,\s]+))?\s*{\s*(?<members>[^}]+)\s*};?", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex DictionaryExtendedParser = new Regex(@"(?<constructor>constructor(\s*\((?<args>.+?)?\))?)(,|$)|(exposed\s*=\s*(?<exposed>(\([^\)]+\))|[^\(\s,\]]+))(,|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex DictionaryMemberParser = new Regex(@"^\s*(\[(?<extended>[^]]+)]\s*)?(((?<required>required)\s+)?(?<type>.+)\s+(?<item>[^=]+)(\s*=\s*(?<value>.+))|(?<type>.+)\s+(?<item>[^=]+))$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex DictionaryMemberExtendedParser = new Regex(@"(?<clamp>clamp)(,|$)|(?<enforcerange>enforcerange)(,|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        public static IEnumerable<DictionaryType> GetAllDictionaries(string cleanString, SpecData specificationData)
        {
            var dictionaries = new List<DictionaryType>();

            foreach (Match dictionaryMatch in DictionaryParser.Matches(cleanString))
            {
                var dictionaryDefinition = new DictionaryType
                {
                    Name = dictionaryMatch.Groups["name"].Value.Trim(),
                    IsPartial = !string.IsNullOrWhiteSpace(dictionaryMatch.Groups["partial"].Value),
                    ExtendedAttribute = CleanString(dictionaryMatch.Groups["extended"].Value),
                    SpecNames = new[] { specificationData.Name }
                };

                var constructors = dictionaryDefinition.Constructors.ToList();
                var exposed = dictionaryDefinition.Exposed.ToList();
                if (!string.IsNullOrWhiteSpace(dictionaryDefinition.ExtendedAttribute))
                {
                    foreach (Match m in DictionaryExtendedParser.Matches(dictionaryDefinition.ExtendedAttribute))
                    {
                        var constructor = m.Groups["constructor"].Value.Trim();
                        if (!string.IsNullOrWhiteSpace(constructor))
                        {
                            constructors.Add(constructor);
                        }
                        var exposedValue = RegexLibrary.GroupingCleaner.Replace(m.Groups["exposed"].Value, string.Empty);
                        if (!string.IsNullOrWhiteSpace(exposedValue))
                        {
                            exposed.AddRange(exposedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()));
                        }
                    }
                }
                dictionaryDefinition.Constructors = constructors.Distinct();
                dictionaryDefinition.Exposed = exposed.Distinct();

                var inherits = dictionaryMatch.Groups["inherits"].Value;
                dictionaryDefinition.Inherits = inherits.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim());
                if (dictionaryMatch.Groups["members"].Length > 0)
                {
                    dictionaryDefinition.Members = GetAllDictionaryMembers(dictionaryMatch.Groups["members"].Value, specificationData);
                }

                if (!dictionaries.Contains(dictionaryDefinition))
                {
                    dictionaries.Add(dictionaryDefinition);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Duplicate dictionary: " + dictionaryDefinition.Name);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return dictionaries;
        }

        private static IEnumerable<DictionaryMember> GetAllDictionaryMembers(string memberItems, SpecData specificationData)
        {
            var memberList = new List<DictionaryMember>();

            memberItems = CleanString(memberItems);

            var members = memberItems.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim()).ToArray();

            foreach (var item in members)
            {
                if (DictionaryMemberParser.IsMatch(item))
                {
                    var m = DictionaryMemberParser.Match(item);

                    var memberItem = new DictionaryMember(m.Groups["item"].Value.Trim())
                    {
                        ExtendedAttribute = m.Groups["extended"].Value.Trim(),
                        Type = RegexLibrary.OldTypeCleaner.Replace(RegexLibrary.TypeCleaner.Replace(m.Groups["type"].Value.Replace("≺", "<").Replace("≻", ">"), "?"), string.Empty).Trim(),
                        Value = m.Groups["value"].Value.Trim(),
                        IsRequired = !string.IsNullOrWhiteSpace(m.Groups["required"].Value.Trim()),
                        SpecNames = new[] { specificationData.Name }
                    };

                    if (!string.IsNullOrWhiteSpace(memberItem.ExtendedAttribute))
                    {
                        foreach (Match mep in DictionaryMemberExtendedParser.Matches(memberItem.ExtendedAttribute))
                        {
                            memberItem.Clamp = memberItem.Clamp || !string.IsNullOrWhiteSpace(mep.Groups["clamp"].Value.Trim());
                            memberItem.EnforceRange = memberItem.EnforceRange || !string.IsNullOrWhiteSpace(mep.Groups["enforcerange"].Value.Trim());
                        }
                    }

                    if (!memberList.Contains(memberItem))
                    {
                        memberList.Add(memberItem);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fail dictionary memember- " + item);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return memberList;
        }
    }
}