using System.Collections.Generic;
using System.Text;

namespace CatalogBuilder.TypeMirrorDataModel
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
        public List<string> SpecNames { get; set; }
        public bool IsDuplicate { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("    confidence: ").Append(Confidence).AppendLine(",");
            sb.Append("    type: ").Append(Type ?? "").AppendLine(",");
            sb.Append("    isPlausiblyInherited: ").Append(IsPlausiblyInherited).AppendLine(",");
            sb.Append("    isPlausiblyDefined: ").Append(IsPlausiblyDefined).AppendLine(",");
            sb.Append("    hasGet: ").Append(HasGet).AppendLine(",");
            sb.Append("    hasSet: ").Append(HasSet).AppendLine(",");
            sb.Append("    isConfigurable: ").Append(IsConfigurable).AppendLine(",");
            sb.Append("    isEnumerable: ").Append(IsEnumerable).AppendLine(",");
            sb.Append("    isWritable: ").Append(IsWritable).AppendLine(",");
            sb.Append("    specNames: ").AppendLine(string.Join(",", SpecNames));

            return sb.ToString();
        }
    }
}
