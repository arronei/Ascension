using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebIDLCollector.IDLTypes
{
    public class NamespaceMember
    {
        public NamespaceMember(string name)
        {
            Name = name;
            Exposed = new List<string>();
            ArgTypes = new List<Argument>();
            SpecNames = new List<string>();
        }

        public Tuple<string, string, IEnumerable<Argument>> Key => new Tuple<string, string, IEnumerable<Argument>>(Name, Type, ArgTypes);

        public string Name { get; }
        public string Type { private get; set; }
        public bool Function { private get; set; }
        public string ExtendedAttribute { get; set; }
        public IEnumerable<string> Exposed { get; set; }
        public bool SecureContext { get; set; }
        public bool HasGet => false;
        public string Args { get; set; }
        public IEnumerable<Argument> ArgTypes { private get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct(bool showSpecName = false)
        {
            var sb = new StringBuilder();
            var comma = string.Empty;

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

                sb.Append("] ");
            }
            sb.Append(Type);
            sb.Append(" ");
            sb.Append(Name);
            if (Function)
            {
                sb.Append("(").Append(Argument.ReconstructArgs(ArgTypes)).Append(")");
            }
            sb.Append(";");
            if (showSpecName && SpecNames.Any())
            {
                sb.Append(" // ").Append(string.Join(", ", SpecNames));
            }

            return Regex.Replace(Regex.Replace(sb.ToString().Trim(), @"\s+;", ";"), @"\s+", " ");
        }

        public override bool Equals(object otherMember)
        {
            var member = otherMember as NamespaceMember;
            return (member != null) && Name.Equals(member.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
