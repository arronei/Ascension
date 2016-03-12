using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebIDLCollector.IDLTypes
{
    public class DictionaryType
    {
        public DictionaryType()
        {
            Constructors = new List<string>();
            Exposed = new List<string>();
            Inherits = new List<string>();
            Members = new List<DictionaryMember>();
            SpecNames = new List<string>();
        }

        public string Name { get; set; }
        public bool IsPartial { get; set; }
        public IEnumerable<string> Inherits { get; set; }
        public IEnumerable<DictionaryMember> Members { get; set; }
        public string ExtendedAttribute { get; set; }
        public IEnumerable<string> Constructors { get; set; }
        public IEnumerable<string> Exposed { get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct(bool showMemberSpecName = false)
        {
            var sb = new StringBuilder();
            var comma = string.Empty;

            sb.AppendLine("// " + string.Join(", ", SpecNames));
            if (Constructors.Any() || Exposed.Any())
            {
                sb.Append("[");
                if (Constructors.Any())
                {
                    sb.Append(string.Join(", ", Constructors));
                    comma = ", ";
                }
                if (Exposed.Any())
                {
                    sb.Append(comma).Append("Exposed=");

                    if (Exposed.Count() == 1)
                    {
                        sb.Append(Exposed.Single());
                    }
                    else
                    {
                        sb.Append("(").Append(string.Join(", ", Exposed)).Append(")");
                    }
                }
                sb.AppendLine("]");
            }
            if (IsPartial)
            {
                sb.Append("partial ");
            }
            sb.Append("dictionary ");
            sb.Append(Name);
            if (Inherits.Any())
            {
                sb.Append(" : ").Append(string.Join(", ", Inherits));
            }
            if (!Members.Any())
            {
                sb.AppendLine(" { }");
            }
            else
            {
                sb.AppendLine().AppendLine("{");

                foreach (var member in Members)
                {
                    sb.Append("    ").AppendLine(member.Reconstruct(showMemberSpecName));
                }

                sb.AppendLine("};");
            }

            return sb.ToString();
        }
    }
}