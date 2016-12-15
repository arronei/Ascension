using System.Collections.Generic;
using System.Text;

namespace WebIDLCollector.TypeMirrorTypes
{
    public class TypeMirrorProperty
    {
        public string Name { private get; set; }
        public string Type { private get; set; }
        public int Confidence { private get; set; }
        private static bool IsPlausiblyInherited => false;
        private static bool IsPlausiblyDefined => false;
        public bool HasGet { private get; set; }
        public bool HasSet { private get; set; }
        public bool IsConfigurable { private get; set; }
        public bool IsEnumerable { private get; set; }
        public bool IsWritable { private get; set; }
        public IEnumerable<string> SpecNames { private get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("\"").Append(Name).AppendLine("\": {");
            sb.Append("\"specNames\": [");
            var comma = string.Empty;
            foreach (var specName in SpecNames)
            {
                sb.Append(comma).Append("\"").Append(specName).Append("\"");
                comma = ", ";
            }
            sb.AppendLine("],");
            sb.Append("\"confidence\": ").Append(Confidence);
            if (!string.IsNullOrWhiteSpace(Type))
            {
                sb.AppendLine(",").Append("\"type\": \"").Append(Type).Append("\"");
            }
            if (IsPlausiblyInherited)
            {
                sb.AppendLine(",").Append("\"isPlausiblyInherited\": ").Append("true");
            }
            if (IsPlausiblyDefined)
            {
                sb.AppendLine(",").Append("\"isPlausiblyDefined\": ").Append("true");
            }
            if (HasGet)
            {
                sb.AppendLine(",").Append("\"hasGet\": ").Append("true");
            }
            if (HasSet)
            {
                sb.AppendLine(",").Append("\"hasSet\": ").Append("true");
            }
            if (IsConfigurable)
            {
                sb.AppendLine(",").Append("\"isConfigurable\": ").Append("true");
            }
            if (IsEnumerable)
            {
                sb.AppendLine(",").Append("\"isEnumerable\": ").Append("true");
            }

            if (Name.Equals("constructor") && IsWritable)
            {
                sb.AppendLine(",").Append("\"isWritable\": ").Append("true");
            }
            sb.AppendLine();
            sb.Append("}");

            return sb.ToString();
        }
    }
}