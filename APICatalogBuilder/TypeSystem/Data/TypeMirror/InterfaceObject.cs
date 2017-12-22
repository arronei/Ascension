using System.Collections.Generic;
using Newtonsoft.Json;

namespace TypeSystem.Data.TypeMirror
{
    public class InterfaceObject
    {
        [JsonProperty("TypeName")]
        public string Name { get; set; }
        public IEnumerable<string> DerivedTypes { get; set; }
        public string BaseType { get; set; }
        public string Confidence { get; set; }
        [JsonProperty("Properties")]
        public IReadOnlyDictionary<string, MemberObject> Members { get; }
        [JsonProperty("SpecNames")]
        public IList<string> SpecificationNames { get; set; }

        public InterfaceObject()
        {
            DerivedTypes = new string[] { };

            Members = new Dictionary<string, MemberObject>();

            SpecificationNames = new List<string>();
        }
    }
}