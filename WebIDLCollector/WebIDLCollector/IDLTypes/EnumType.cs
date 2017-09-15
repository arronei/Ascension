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
        public IEnumerable<string> EnumValues { private get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct(bool showMemberSpecName = false)
        {
            var sb = new StringBuilder();

            if (showMemberSpecName)
            {
                sb.Append("// ").AppendLine(string.Join(", ", SpecNames));
            }

            sb.Append("enum ").AppendLine(Name);
            sb.Append("{");
            var comma = string.Empty;
            foreach (var item in EnumValues)
            {
                sb.AppendLine(comma).Append("    \"").Append(item).Append("\"");
                comma = ",";
            }
            sb.AppendLine();
            sb.AppendLine("};");

            return sb.ToString();
        }
    }
}