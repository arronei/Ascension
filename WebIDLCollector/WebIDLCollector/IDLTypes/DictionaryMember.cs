using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebIDLCollector.IDLTypes
{
    public class DictionaryMember
    {
        public DictionaryMember(string name)
        {
            Name = name;
            SpecNames = new List<string>();
        }

        public string ExtendedAttribute { get; set; }
        public bool Clamp { get; set; }
        public bool EnforceRange { get; set; }
        public string Name { get; }
        public string Type { get; set; }
        public string Value { get; set; }
        public bool IsRequired { get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct(bool showSpecName = false)
        {
            var sb = new StringBuilder();
            var comma = string.Empty;

            if (Clamp || EnforceRange)
            {
                sb.Append("[");

                if (Clamp)
                {
                    sb.Append("Clamp");
                    comma = ", ";
                }
                if (EnforceRange)
                {
                    sb.Append(comma).Append("EnforceRange");
                }

                sb.Append("] ");
            }

            if (IsRequired)
            {
                sb.Append("required ");
            }

            sb.Append(Type.Trim());
            sb.Append(" ");
            sb.Append(Name.Trim());
            if (!string.IsNullOrWhiteSpace(Value))
            {
                sb.Append(" = ");
                sb.Append(Value.Trim());
            }
            sb.Append(";");

            if (!showSpecName || !SpecNames.Any())
            {
                return sb.ToString();
            }

            sb.Append(" // ");
            sb.AppendLine(string.Join(", ", SpecNames));

            return sb.ToString();
        }

        public string Reconstruct2(bool showSpecName = false)
        {
            return (IsRequired ? "required " : string.Empty) + Regex.Replace((Type + " " + Name).Trim(), @"\s+", " ") + (!string.IsNullOrWhiteSpace(Value) ? " = " + Value : string.Empty).TrimEnd() + ";" + (showSpecName ? (SpecNames.Any() ? " // " + string.Join(", ", SpecNames) : string.Empty) : string.Empty);
        }

        public override bool Equals(object otherMember)
        {
            var member = otherMember as DictionaryMember;
            return (member != null) && Name.Equals(member.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public class DictionaryMemberCompare : IEqualityComparer<DictionaryMember>
    {
        public bool Equals(DictionaryMember x, DictionaryMember y)
        {
            return x.Name.Equals(y.Name) && x.Type.Equals(y.Type);
        }

        public int GetHashCode(DictionaryMember obj)
        {
            return ((obj.Name.GetHashCode() << 5)
                    ^ obj.Type.GetHashCode()) << 5;
        }
    }
}