using System.Collections.Generic;

namespace MS.Internal.TypeMirror.Core
{
    public class PropertyObject
    {
        public string Confidence { get; set; }
        public string Type { get; set; }
        public string IsPlausiblyInherited { get; set; }
        public string IsPlausiblyDefined { get; set; }
        public string HasGet { get; set; }
        public string HasSet { get; set; }
        public string IsConfigurable { get; set; }
        public string IsEnumerable { get; set; }
        public string IsWritable { get; set; }
        public IList<string> SpecificationNames { get; }
        public bool IsDuplicate { get; set; }

        public PropertyObject()
        {
            SpecificationNames = new List<string>();
        }
    }
}