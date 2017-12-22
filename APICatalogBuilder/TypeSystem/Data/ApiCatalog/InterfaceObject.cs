using System.Collections.Generic;

namespace TypeSystem.Data.ApiCatalog
{
    public class InterfaceObject
    {
        public string Name { get; }

        public SortedDictionary<string, MemberObject> Members { get; }

        public IList<string> SpecificationNames { get; set; }

        public IList<string> SupportedBrowsers { get; set; }

        public InterfaceObject(string interfaceName)
        {
            Name = interfaceName;
            Members = new SortedDictionary<string, MemberObject>();
            SpecificationNames = new List<string>();
            SupportedBrowsers = new List<string>();
        }
    }
}