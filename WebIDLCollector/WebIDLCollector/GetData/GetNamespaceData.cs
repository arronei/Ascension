using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.GetData
{
    public partial class DataCollectors
    {
        private static readonly Regex NamespaceParser = new Regex(@"(\[(?<extended>[^\]]+)\]\s*)?
        ((?<partial>partial)\s+)?(/\*[^\*]+\*/\s*)?namespace\s+(/\*[^\*]+\*/\s*)?
        (?<name>\w+)\s*\{\s*(?<members>([^\}]*?\{.*?\};.*?|[^\}]+))?\s*\};?", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex NamespaceExtendedParser = new Regex(@"((exposed(\s*=\s*(?<exposed>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<securecontext>securecontext)(,|$))+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex NamespaceMember = new Regex(@"^\s*(\[(?<extended>[^\]]+)]\s*)?
        (?<type>.+)\s+(?<item>[^\(\s]+)?\s*(?<function>\((?<args>.*)\))$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex NamespaceMemberExtendedParser = new Regex(@"((exposed(\s*=\s*(?<exposed>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<securecontext>securecontext)(,|$))+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        public static IEnumerable<NamespaceType> GetAllNamespaces(string cleanString, SpecData specificationData)
        {
            var namespaces = new List<NamespaceType>();

            foreach (Match _namespace in NamespaceParser.Matches(cleanString))
            {

            }

            return namespaces;
        }
    }
}