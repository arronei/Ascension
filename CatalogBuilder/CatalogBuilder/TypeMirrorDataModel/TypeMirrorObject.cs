using System.Collections.Generic;
using System.Text;

namespace CatalogBuilder.TypeMirrorDataModel
{
    public class TypeMirrorObject
    {
        public string BrowserVersion { get; set; }
        public string Timestamp { get; set; }
        public SortedDictionary<string, TypeObject> Types { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("browserVersion: ").Append(BrowserVersion).AppendLine(",");
            sb.Append("timestamp: ").Append(Timestamp).AppendLine(",");
            sb.AppendLine("types: {");

            foreach (var type in Types)
            {
                sb.Append(type.Key).AppendLine(": {");
                sb.Append(type.Value);
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        public TypeObject GetTypeObject(string typeName)
        {
            return Types.ContainsKey(typeName) ? Types[typeName] : null;
        }
    }
}