using System.Collections.Generic;

namespace MS.Internal.TypeSystem.Core
{
    public class InterfaceObject
    {
        public string Name { get; }

        public IReadOnlyDictionary<string, MemberObject> Members { get; }

        public IList<string> SpecificationNames { get; }

        public Browsers SupportedBrowsers { get; set; }

        public InterfaceObject(string interfaceName)
        {
            Name = interfaceName;
            Members = new SortedDictionary<string, MemberObject>();
            SpecificationNames = new List<string>();
        }
    }
}