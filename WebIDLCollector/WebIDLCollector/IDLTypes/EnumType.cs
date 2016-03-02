using System.Collections.Generic;
using System.Text;

namespace WebIDLCollector.IDLTypes
{
    public class EnumType
    {
        public EnumType()
        {
            EnumValues = new List<string>();
            SpecNames = new List<string>();
        }

        public string Name { get; set; }
        public IEnumerable<string> EnumValues { get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append("// ").AppendLine(string.Join(", ", SpecNames));
                sb.Append("enum ").AppendLine(Name);
                sb.AppendLine("{");
                var comma = string.Empty;
                foreach (var item in EnumValues)
                {
                    sb.Append(comma).Append("    \"").Append(item).Append("\"");
                    comma = ",\r\n";
                }
                sb.AppendLine();
                sb.AppendLine("};");
                return sb.ToString();
            }
        }
    }
}