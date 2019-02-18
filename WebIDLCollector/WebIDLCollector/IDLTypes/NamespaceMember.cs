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
        public string Type { get; set; }
        public bool Function { private get; set; }
        public bool Attribute { private get; set; }
        public string ExtendedAttribute { get; set; }
        public IEnumerable<string> Exposed { get; set; }
        public bool AllowShared { get; set; }
        public bool CeReactions { get; set; }
        public bool Clamp { get; set; }
        public bool Default { get; set; }
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
        public bool Unscopable { get; set; }

        public bool Pure { get; set; }
        public bool Constant { get; set; }
        public string StoreInSlot { get; set; }

        public bool HasGet { get; set; }
        public bool HasSet { get; set; }
        public bool Readonly { get; set; }
        public string Bracket { private get; set; }
        public string Value { private get; set; }
        public string Args { get; set; }
        public IEnumerable<Argument> ArgTypes { private get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct(bool showSpecName = false)
        {
            var sb = new StringBuilder();
            var comma = string.Empty;

            if (AllowShared ||
                CeReactions ||
                Clamp ||
                Default ||
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
                Unscopable ||
                Pure ||
                Constant ||
                !string.IsNullOrWhiteSpace(StoreInSlot)
                )
            {
                sb.Append("[");

                if (Clamp)
                {
                    sb.Append("AllowShared");
                    comma = ", ";
                }
                if (CeReactions)
                {
                    sb.Append("CEReactions");
                    comma = ", ";
                }
                if (Clamp)
                {
                    sb.Append("Clamp");
                    comma = ", ";
                }
                if (Default)
                {
                    sb.Append("Default");
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
                if (Unscopable)
                {
                    sb.Append(comma).Append("Unscopable");
                    comma = ", ";
                }
                if (Pure)
                {
                    sb.Append(comma).Append("Pure");
                }
                if (Constant)
                {
                    sb.Append(comma).Append("Constant");
                }
                if (!string.IsNullOrWhiteSpace(StoreInSlot))
                {
                    sb.Append(comma).Append("StoreInSlot=").Append(StoreInSlot);
                    comma = ", ";
                }

                sb.Append("] ");
            }

            if (Readonly)
            {
                sb.Append("readonly ");
            }
            if (Attribute)
            {
                sb.Append("attribute ");
            }
            sb.Append(Type);
            sb.Append(" ");
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
            var member = otherMember as NamespaceMember;
            return (member != null) && Name.Equals(member.Name) && Type.Equals(member.Type) && ArgTypes.SequenceEqual(member.ArgTypes, new ArgumentTypeCompare());
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public class NamespaceMemberCompare : IEqualityComparer<NamespaceMember>
    {
        public bool Equals(NamespaceMember x, NamespaceMember y)
        {
            return x.Key.Item1.Equals(y.Key.Item1) && x.Key.Item2.Equals(y.Key.Item2) && (x.Key.Item3 == null || x.Key.Item3.SequenceEqual(y.Key.Item3, new ArgumentTypeCompare()));
        }

        public int GetHashCode(NamespaceMember obj)
        {
            return ((obj.Key.Item1.GetHashCode() << 5)
                    ^ obj.Key.Item2.GetHashCode()) << 5;
        }
    }
}
