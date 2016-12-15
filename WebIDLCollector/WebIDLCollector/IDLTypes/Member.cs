using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebIDLCollector.IDLTypes
{
    public class Member
    {
        public Member(string name)
        {
            Name = name;
            Exposed = new List<string>();
            ArgTypes = new List<Argument>();
            SpecNames = new List<string>();
        }

        public Tuple<string, string, IEnumerable<Argument>> Key => new Tuple<string, string, IEnumerable<Argument>>(Name, Type, ArgTypes);

        public string Name { get; }
        public string Type { get; set; }
        public bool Function { private get; set; }
        public bool Attribute { private get; set; }
        public string ExtendedAttribute { get; set; }
        public IEnumerable<string> Exposed { get; set; }
        public bool Clamp { get; set; }
        public bool EnforceRange { get; set; }
        public bool LenientSetter { get; set; }
        public bool LenientThis { get; set; }
        public bool NewObject { get; set; }
        public string PutForwards { get; set; }
        public bool Replaceable { get; set; }
        public bool SameObject { get; set; }
        public bool SecureContext { get; set; }
        public string TreatNullAs { get; set; }
        public string TreatUndefinedAs { get; set; }
        public bool Unforgeable { get; set; }
        public bool Unscopeable { get; set; }
        public bool Static { private get; set; }
        public bool HasGet { get; set; }
        public bool HasSet { get; set; }
        public bool Getter { get; set; }
        public bool Setter { private get; set; }
        public bool Creator { private get; set; }
        public bool Deleter { private get; set; }
        public bool LegacyCaller { private get; set; }
        public bool Stringifier { get; set; }
        public bool Serializer { get; set; }
        public bool Inherit { private get; set; }
        public bool MapLike { get; set; }
        public bool SetLike { get; set; }
        public bool Readonly { get; set; }
        public bool Required { private get; set; }
        public bool Const { private get; set; }
        public bool Iterable { get; set; }
        public bool LegacyIterable { private get; set; }
        public string Bracket { private get; set; }
        public string Value { private get; set; }
        public string Args { get; set; }
        public IEnumerable<Argument> ArgTypes { private get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct(bool showSpecName = false)
        {
            var sb = new StringBuilder();
            var comma = string.Empty;

            if (Clamp ||
                EnforceRange ||
                Exposed.Any() ||
                LenientSetter ||
                LenientThis ||
                NewObject ||
                !string.IsNullOrWhiteSpace(PutForwards) ||
                Replaceable ||
                SameObject ||
                SecureContext ||
                !string.IsNullOrWhiteSpace(TreatNullAs) ||
                !string.IsNullOrWhiteSpace(TreatUndefinedAs) ||
                Unforgeable ||
                Unscopeable)
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
                    comma = ", ";
                }
                if (LenientSetter)
                {
                    sb.Append(comma).Append("LenientSetter");
                    comma = ", ";
                }
                if (LenientThis)
                {
                    sb.Append(comma).Append("LenientThis");
                    comma = ", ";
                }
                if (NewObject)
                {
                    sb.Append(comma).Append("NewObject");
                    comma = ", ";
                }
                if (!string.IsNullOrWhiteSpace(PutForwards))
                {
                    sb.Append(comma).Append("PutForwards=").Append(PutForwards);
                    comma = ", ";
                }
                if (Replaceable)
                {
                    sb.Append(comma).Append("Replaceable");
                    comma = ", ";
                }
                if (SameObject)
                {
                    sb.Append(comma).Append("SameObject");
                    comma = ", ";
                }
                if (SecureContext)
                {
                    sb.Append(comma).Append("SecureContext");
                    comma = ", ";
                }
                if (!string.IsNullOrWhiteSpace(TreatNullAs))
                {
                    sb.Append(comma).Append("TreatNullAs=").Append(TreatNullAs);
                    comma = ", ";
                }
                if (Unforgeable)
                {
                    sb.Append(comma).Append("Unforgeable");
                    comma = ", ";
                }
                if (Unscopeable)
                {
                    sb.Append(comma).Append("Unscopeable");
                }

                sb.Append("] ");
            }

            if (Static)
            {
                sb.Append("static ");
            }
            if (Getter)
            {
                sb.Append("getter ");
            }
            if (Setter)
            {
                sb.Append("setter ");
            }
            if (Creator)
            {
                sb.Append("creator ");
            }
            if (Deleter)
            {
                sb.Append("deleter ");
            }
            if (LegacyCaller)
            {
                sb.Append("legacycaller ");
            }
            if (Stringifier)
            {
                sb.Append("stringifier ");
            }
            if (Serializer)
            {
                sb.Append("serializer ");
            }
            if (Inherit)
            {
                sb.Append("inherit ");
            }
            if (Readonly)
            {
                sb.Append("readonly ");
            }
            if (Const)
            {
                sb.Append("const ");
            }
            if (Attribute)
            {
                sb.Append("attribute ");
            }
            if (Iterable)
            {
                sb.Append("iterable ");
            }
            if (LegacyIterable)
            {
                sb.Append("legacyiterable ");
            }
            if (MapLike)
            {
                sb.Append("maplike <");
            }
            if (SetLike)
            {
                sb.Append("setlike <");
            }
            sb.Append(Type);
            if (MapLike || SetLike)
            {
                sb.Append(">");
            }
            sb.Append(" ");
            if (Required)
            {
                sb.Append("required ");
            }
            sb.Append(Name);
            if (Function)
            {
                sb.Append("(").Append(Argument.ReconstructArgs(ArgTypes)).Append(")");
            }
            if (!string.IsNullOrWhiteSpace(Value))
            {
                sb.Append(" = ");
                if (!string.IsNullOrWhiteSpace(Bracket))
                {
                    sb.Append(Bracket).Append(" ");
                }

                sb.Append(Value);
                if (!string.IsNullOrWhiteSpace(Bracket))
                {
                    sb.Append(Bracket.Equals("{") ? " }" : " ]");
                }
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
            var member = otherMember as Member;
            return (member != null) && Name.Equals(member.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public class MemberCompare : IEqualityComparer<Member>
    {
        public bool Equals(Member x, Member y)
        {
            return x.Key.Item1.Equals(y.Key.Item1) && x.Key.Item2.Equals(y.Key.Item2) && (x.Key.Item3 == null || x.Key.Item3.SequenceEqual(y.Key.Item3, new ArgumentTypeCompare()));
        }

        public int GetHashCode(Member obj)
        {
            return ((obj.Key.Item1.GetHashCode() << 5)
                    ^ obj.Key.Item2.GetHashCode()) << 5;
        }
    }
}