using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public Tuple<string, string, IEnumerable<Argument>> Key
        {
            get { return new Tuple<string, string, IEnumerable<Argument>>(Name, Type, ArgTypes); }
        }

        public string Name { get; }
        public string Type { get; set; }
        public bool Function { get; set; }
        public bool Attribute { get; set; }
        public string ExtendedAttribute { get; set; }
        public IEnumerable<string> Exposed { get; set; }
        public bool Clamp { get; set; }
        public bool EnforceRange { get; set; }
        public bool LenientThis { get; set; }
        public bool NewObject { get; set; }
        public string PutForwards { get; set; }
        public bool Replaceable { get; set; }
        public bool SameObject { get; set; }
        public string TreatNullAs { get; set; }
        public bool Unforgeable { get; set; }
        public bool Unscopeable { get; set; }
        public bool Static { get; set; }
        public bool HasGet { get; set; }
        public bool HasSet { get; set; }
        public bool Getter { get; set; }
        public bool Setter { get; set; }
        public bool Creator { get; set; }
        public bool Deleter { get; set; }
        public bool LegacyCaller { get; set; }
        public bool Stringifier { get; set; }
        public bool Serializer { get; set; }
        public bool Inherit { get; set; }
        public bool MapLike { get; set; }
        public bool SetLike { get; set; }
        public bool Readonly { get; set; }
        public bool Required { get; set; }
        public bool Const { get; set; }
        public bool Iterable { get; set; }
        public bool LegacyIterable { get; set; }
        public string Value { get; set; }
        public string Args { get; set; }
        public IEnumerable<Argument> ArgTypes { get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct2(bool showSpecName = false)
        {
            var sb = new StringBuilder();
            var comma = string.Empty;

            return sb.ToString();
        }


        public string Reconstruct(bool showSpecName = false)
        {
            return
                (
                    (!string.IsNullOrWhiteSpace(ExtendedAttribute) ? "[" + ExtendedAttribute + "] " : string.Empty) +
                    (Static ? "static " : string.Empty) +
                    (Getter ? "getter " : string.Empty) +
                    (Setter ? "setter " : string.Empty) +
                    (Creator ? "creator " : string.Empty) +
                    (Deleter ? "deleter " : string.Empty) +
                    (LegacyCaller ? "legacycaller " : string.Empty) +
                    (Stringifier ? "stringifier " : string.Empty) +
                    (Serializer ? "serializer " : string.Empty) +
                    (Inherit ? "inherit " : string.Empty) +
                    (Readonly ? "readonly " : string.Empty) +
                    (Const ? "const " : string.Empty) +
                    (Attribute ? "attribute " : string.Empty) +
                    (Iterable ? "iterable " : string.Empty) +
                    (LegacyIterable ? "legacyiterable " : string.Empty) +
                    (MapLike ? "maplike <" : string.Empty) +
                    (SetLike ? "setlike <" : string.Empty) +
                    Type + " " +
                    (MapLike || SetLike ? ">" : string.Empty) +
                    (Required ? "required " : string.Empty) +
                    Name +
                    (Function ? "(" + ReconstructArgs(ArgTypes) + ")" : (!string.IsNullOrWhiteSpace(Value) ? " = " + Value : string.Empty)) +
                    ";"
                    + (showSpecName ? (SpecNames.Any() ? " // " + string.Join(", ", SpecNames) : string.Empty) : string.Empty)
                ).TrimEnd();
        }

        private static string ReconstructArgs(IEnumerable<Argument> argTypes)
        {
            var sb = new StringBuilder();

            var comma = string.Empty;
            foreach (var argument in argTypes)
            {
                sb.Append(comma).Append(argument.Reconstruct);
                comma = ", ";
            }

            return sb.ToString();
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