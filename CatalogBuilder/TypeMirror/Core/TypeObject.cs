using System.Collections.Generic;

namespace MS.Internal.TypeMirror.Core
{
    public class TypeObject
    {
        public string TypeName { get; set; }
        public IEnumerable<string> DerivedTypes { get; set; }
        public string BaseType { get; set; }
        public string Confidence { get; set; }
        public IReadOnlyDictionary<string, PropertyObject> Properties { get; }
        public IList<string> SpecificationNames { get; set; }

        public TypeObject()
        {
            DerivedTypes = new string[] { };

            Properties = new SortedDictionary<string, PropertyObject>();

            SpecificationNames = new List<string>();
        }
    }
}