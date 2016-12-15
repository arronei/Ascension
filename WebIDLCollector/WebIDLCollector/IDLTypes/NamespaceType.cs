using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebIDLCollector.IDLTypes
{
    public class NamespaceType
    {
        public NamespaceType()
        {
            Exposed = new List<string>();
        }

        public string ExtendedAttribute { get; set; }
        public IEnumerable<string> Exposed { get; set; }
        public bool SecureContext { get; set; }
        public bool IsPartial { private get; set; }
        public string Name { get; set; }
        public IEnumerable<NamespaceMember> Members { get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct(bool showMemberSpecName = false)
        {
            var sb = new StringBuilder();
            var comma = string.Empty;

            sb.AppendLine("// " + string.Join(", ", SpecNames));

            if (Exposed.Any() ||
                SecureContext)
            {
                sb.Append("[");
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
                    comma = ", ";
                }
                if (SecureContext)
                {
                    sb.Append(comma).Append("SecureContext");
                }
                sb.AppendLine("]");
            }
            if (IsPartial)
            {
                sb.Append("partial ");
            }
            sb.Append("namespace ");
            sb.Append(Name);
            if (!Members.Any())
            {
                sb.AppendLine(" { };");
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

        public class NamespaceNameCompare : IEqualityComparer<NamespaceType>
        {
            public bool Equals(NamespaceType x, NamespaceType y)
            {
                return x.Name.Equals(y.Name);
            }

            public int GetHashCode(NamespaceType obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}