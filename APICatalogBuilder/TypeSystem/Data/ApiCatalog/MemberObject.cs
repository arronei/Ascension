using System.Collections.Generic;

namespace TypeSystem.Data.ApiCatalog
{
    public class MemberObject
    {
        public string Name { get; }

        public IList<string> SpecificationNames { get; set; }

        public IList<string> SupportedBrowsers { get; set; }

        public MemberObject(string memberName)
        {
            Name = memberName;
            SpecificationNames = new List<string>();
            SupportedBrowsers = new List<string>();
        }
    }
}