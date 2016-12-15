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
        private static readonly Regex NamespaceParser = new Regex(@"(\[(?<extended>[^\]]+)\]\s*)?
        ((?<partial>partial)\s+)?(/\*[^\*]+\*/\s*)?namespace\s+(/\*[^\*]+\*/\s*)?
        (?<name>\w+)\s*\{\s*(?<members>([^\}]*?\{.*?\};.*?|[^\}]+))?\s*\};?", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex NamespaceExtendedParser = new Regex(@"((exposed(\s*=\s*(?<exposed>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<securecontext>securecontext)(,|$))+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex IndividualNamespaceMember = new Regex(@"^\s*(\[(?<extended>[^\]]+)]\s*)?
        (?<type>.+)\s+(?<item>[^\(\s]+)?\s*(?<function>\((?<args>.*)\))$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex NamespaceMemberExtendedParser = new Regex(@"((exposed(\s*=\s*(?<exposed>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<securecontext>securecontext)(,|$))+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        public static IEnumerable<NamespaceType> GetAllNamespaces(string cleanString, SpecData specificationData)
        {
            var namespaces = new List<NamespaceType>();

            foreach (Match _namespace in NamespaceParser.Matches(cleanString))
            {
                var namespaceDefinition = new NamespaceType
                {
                    Name = _namespace.Groups["name"].Value.Trim(),
                    IsPartial = !string.IsNullOrWhiteSpace(_namespace.Groups["partial"].Value.Trim()),
                    ExtendedAttribute = CleanString(_namespace.Groups["extended"].Value),
                    SpecNames = new[] { specificationData.Name }
                };

                if (!string.IsNullOrWhiteSpace(namespaceDefinition.ExtendedAttribute))
                {
                    var exposed = namespaceDefinition.Exposed.ToList();

                    foreach (Match m in NamespaceExtendedParser.Matches(namespaceDefinition.ExtendedAttribute))
                    {
                        namespaceDefinition.SecureContext = namespaceDefinition.SecureContext || !string.IsNullOrWhiteSpace(m.Groups["securecontext"].Value.Trim());

                        var exposedValue = RegexLibrary.GroupingCleaner.Replace(m.Groups["exposed"].Value, string.Empty);
                        if (!string.IsNullOrWhiteSpace(exposedValue))
                        {
                            exposed.AddRange(exposedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim()));
                        }
                    }

                    namespaceDefinition.Exposed = exposed.Distinct();
                }

                namespaceDefinition.Members = _namespace.Groups["members"].Length > 0 ? GetAllNamespaceMembers(_namespace.Groups["members"].Value, specificationData) : new List<NamespaceMember>();

                if (!namespaces.Contains(namespaceDefinition))
                {
                    namespaces.Add(namespaceDefinition);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Duplicate namespace: " + namespaceDefinition.Name);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return namespaces;
        }

        private static IEnumerable<NamespaceMember> GetAllNamespaceMembers(string memberItems, SpecData specificationData)
        {
            var memberList = new List<NamespaceMember>();

            memberItems = CleanString(memberItems);

            var members = memberItems.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim()).ToArray();

            foreach (var item in members.Where(item => !string.IsNullOrWhiteSpace(item)))
            {
                if (!IndividualNamespaceMember.IsMatch(item))
                {
                    continue;
                }
                var m = IndividualNamespaceMember.Match(item);

                var name = m.Groups["item"].Value.Trim();
                //Skip special CSSOM definitions
                if (new[] { "_camel_cased_attribute", "_webkit_cased_attribute", "_dashed_attribute" }.Contains(name))
                {
                    continue;
                }

                var memberItem = new NamespaceMember(name)
                {
                    Type = RegexLibrary.OldTypeCleaner.Replace(RegexLibrary.TypeCleaner.Replace(m.Groups["type"].Value.Replace("≺", "<").Replace("≻", ">"), "?"), string.Empty).Trim(),
                    Args = m.Groups["args"].Value.Trim(),
                    Function = !string.IsNullOrWhiteSpace(m.Groups["function"].Value),
                    ExtendedAttribute = m.Groups["extended"].Value.Trim(),
                    SpecNames = new[] { specificationData.Name }
                };

                if (!string.IsNullOrWhiteSpace(memberItem.ExtendedAttribute))
                {
                    var exposed = new List<string>();
                    foreach (Match mep in NamespaceMemberExtendedParser.Matches(memberItem.ExtendedAttribute))
                    {
                        memberItem.SecureContext = memberItem.SecureContext || !string.IsNullOrWhiteSpace(mep.Groups["securecontext"].Value.Trim());

                        var exposedValue = RegexLibrary.GroupingCleaner.Replace(mep.Groups["exposed"].Value, string.Empty);
                        if (!string.IsNullOrWhiteSpace(exposedValue))
                        {
                            exposed.AddRange(exposedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim()));
                        }
                    }
                    memberItem.Exposed = exposed.Distinct();
                }

                try
                {
                    memberItem.ArgTypes = GetArgTypes(memberItem.Args);
                }
                catch (ArgumentException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fail argument for namespace member- " + item);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    continue;
                }

                if (!memberList.Contains(memberItem))
                {
                    memberList.Add(memberItem);
                }
            }

            return memberList.OrderBy(a => a.Name).ToList();
        }
    }
}