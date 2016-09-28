using System.Collections.Generic;

namespace MS.Internal.TypeSystem.Core
{
    public class MemberObject
    {
        public string Name { get; }

        public IList<string> SpecificationNames { get; }

        public Browsers SupportedBrowsers { get; set; }

        public MemberObject(string memberName)
        {
            Name = memberName;
            SpecificationNames = new List<string>();
        }
    }
}