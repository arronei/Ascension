using System.Collections.Generic;
using System.Text;

namespace WebIDLCollector.IDLTypes
{
    public class CallbackType
    {
        public CallbackType(string name)
        {
            Name = name;
            ArgTypes = new List<Argument>();
            SpecNames = new List<string>();
        }

        public string Name { get; }
        public string Type { private get; set; }
        public string Args { get; set; }
        public IEnumerable<Argument> ArgTypes { private get; set; }
        public string ExtendedAttribute { get; set; }
        public bool TreatNonObjectAsNull { get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct()
        {
            var sb = new StringBuilder();

            if (TreatNonObjectAsNull)
            {
                sb.Append("[TreatNonObjectAsNull] ");
            }
            sb.Append("callback ");
            sb.Append(Name);
            sb.Append(" = ");
            sb.Append(Type);
            sb.Append(" (");
            sb.Append(Argument.ReconstructArgs(ArgTypes));
            sb.Append(");");

            sb.Append(" // ");
            sb.AppendLine(string.Join(", ", SpecNames));

            return sb.ToString();
        }
    }
}