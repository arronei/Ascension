using System.Collections.Generic;
using System.Text;

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

        public string Reconstruct()
        {
            var sb = new StringBuilder();

            sb.Append("typedef ");
            sb.Append(Type);
            sb.Append(" ");
            sb.Append(Name);
            sb.Append("; // ");
            sb.AppendLine(string.Join(", ", SpecNames));

            return sb.ToString();
        }
    }
}