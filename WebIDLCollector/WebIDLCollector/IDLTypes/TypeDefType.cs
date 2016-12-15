using System.Collections.Generic;

namespace WebIDLCollector.IDLTypes
{
    public class TypeDefType
    {
        public TypeDefType()
        {
            SpecNames = new List<string>();
        }

        public string Name { get; set; }
        public string Type { private get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct => "typedef " + Type + " " + Name + "; // " + string.Join(", ", SpecNames);
    }
}