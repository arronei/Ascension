using System.Collections.Generic;
using System.Text;

namespace WebIDLCollector.TypeMirrorTypes
{
    public class TypeMirrorType
    {
        public string TypeName { get; set; }
        public List<string> DerivedTypes { get; set; }
        public string BaseType { get; set; }
        public int Confidence { get; set; }
        public List<TypeMirrorProperty> Properties { get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("\"").Append(TypeName).AppendLine("\": {");
            sb.Append("\"typeName\": \"").Append(TypeName).AppendLine("\",");

            sb.Append("\"derivedTypes\": [");
            if (DerivedTypes.Count > 0)
            {
                // add derivedTypes Array
                sb.Append("\"").Append(string.Join(", ", DerivedTypes)).Append("\"");
            }
            sb.AppendLine("],");

            if (!string.IsNullOrWhiteSpace(BaseType))
            {
                sb.Append("\"baseType\": \"").Append(BaseType).AppendLine("\",");
            }
            sb.Append("\"confidence\": ").Append(Confidence).AppendLine(",");

            sb.Append("\"specNames\": \"").Append(string.Join(", ", SpecNames)).AppendLine("\"");

            sb.Append("\"properties\": {");
            if (Properties.Count > 0)
            {
                // add list of members
                sb.AppendLine().Append(string.Join(",\r\n", Properties)).AppendLine();
            }
            sb.AppendLine("},");
            sb.Append("}");

            return sb.ToString();
        }
    }
}