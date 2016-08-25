using System.Collections.Generic;
using System.Text;

namespace CatalogBuilder.TypeMirrorDataModel
{
    public class TypeObject
    {
        public string TypeName { get; set; }
        public string[] DerivedTypes { get; set; }
        public string BaseType { get; set; }
        public string Confidence { get; set; }
        public SortedDictionary<string, PropertyObject> Properties { get; set; }
        public List<string> SpecNames { get; set; }

        public override string ToString()
        {

            var sb = new StringBuilder();
            sb.Append("typeName: ").Append(TypeName).AppendLine(",");
            sb.Append("derivedType: [");
            var comma = string.Empty;
            foreach (var d in DerivedTypes)
            {
                sb.Append(comma);
                comma = ", ";
                sb.Append(d);
            }
            sb.AppendLine("],");

            sb.Append("baseType: ").Append(BaseType).AppendLine(",");
            sb.Append("confidence: ").Append(Confidence).AppendLine(",");
            sb.AppendLine("properties: {");
            foreach (var property in Properties)
            {
                var propertyName = property.Key;
                sb.Append("  ").Append(propertyName).AppendLine(": {");

                var p = property.Value;
                sb.Append(p);
                sb.AppendLine("  }");
            }
            return sb.ToString();
        }

        public PropertyObject GetPropertyObject(string propertyName)
        {
            return Properties.ContainsKey(propertyName) ? Properties[propertyName] : null;
        }

        public bool HasParentType()
        {
            return BaseType != null;
        }

        public bool HasProperty(string propertyName)
        {
            return Properties.ContainsKey(propertyName);
        }
    }
}