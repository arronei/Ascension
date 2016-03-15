using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebIDLCollector.IDLTypes
{
    public class InterfaceType
    {
        public InterfaceType()
        {
            Constructors = new List<string>();
            Exposed = new List<string>();
            Globals = new List<string>();
            PrimaryGlobals = new List<string>();
            NamedConstructors = new List<string>();
            Inherits = new List<string>();
            ExtendedBy = new List<string>();
            Members = new List<Member>();
            SpecNames = new List<string>();
        }

        public string ExtendedAttribute { get; set; }
        public IEnumerable<string> Constructors { get; set; }
        public IEnumerable<string> Exposed { get; set; }
        public bool IsGlobal { get; set; }
        public IEnumerable<string> Globals { get; set; }
        public bool ImplicitThis { get; set; }
        public bool ArrayClass { get; set; }
        public bool LegacyArrayClass { get; set; }
        public bool LegacyUnenumerableNamedProperties { get; set; }
        public IEnumerable<string> NamedConstructors { get; set; }
        public bool NamedPropertiesObject { get; set; }
        public bool NoInterfaceObject { get; set; }
        public bool OverrideBuiltins { get; set; }
        public bool IsPrimaryGlobal { get; set; }
        public IEnumerable<string> PrimaryGlobals { get; set; }
        public bool Unforgeable { get; set; }
        public bool IsPartial { get; set; }
        public bool IsCallback { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Inherits { get; set; }
        public IEnumerable<string> ExtendedBy { get; set; }
        public IEnumerable<Member> Members { get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct(bool showMemberSpecName = false)
        {
            var sb = new StringBuilder();
            var comma = string.Empty;

            sb.AppendLine("// " + string.Join(", ", SpecNames));
            if (Constructors.Any() ||
                NamedConstructors.Any() ||
                Exposed.Any() ||
                IsGlobal ||
                Globals.Any() ||
                ImplicitThis ||
                ArrayClass ||
                LegacyArrayClass ||
                LegacyUnenumerableNamedProperties ||
                NoInterfaceObject ||
                OverrideBuiltins ||
                IsPrimaryGlobal ||
                PrimaryGlobals.Any() ||
                Unforgeable)
            {
                sb.Append("[");
                if (IsGlobal)
                {
                    sb.Append("Global");
                    if (Globals.Any())
                    {
                        sb.Append("=");
                        if (Globals.Count() == 1)
                        {
                            sb.Append(Globals.Single());
                        }
                        else
                        {
                            sb.Append("(").Append(string.Join(", ", Globals)).Append(")");
                        }
                    }
                    comma = ", ";
                }
                if (IsPrimaryGlobal)
                {
                    sb.Append("PrimaryGlobal");
                    if (PrimaryGlobals.Any())
                    {
                        sb.Append("=");
                        if (PrimaryGlobals.Count() == 1)
                        {
                            sb.Append(PrimaryGlobals.Single());
                        }
                        else
                        {
                            sb.Append("(").Append(string.Join(", ", PrimaryGlobals)).Append(")");
                        }
                    }
                    comma = ", ";
                }
                if (Constructors.Any())
                {
                    sb.Append(comma).Append(string.Join(", ", Constructors));
                    comma = ", ";
                }
                if (NamedConstructors.Any())
                {
                    sb.Append(comma).Append(string.Join(", ", NamedConstructors));
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
                if (ImplicitThis)
                {
                    sb.Append(comma).Append("ImplicitThis");
                    comma = ", ";
                }
                if (ArrayClass)
                {
                    sb.Append(comma).Append("ArrayClass");
                    comma = ", ";
                }
                if (LegacyArrayClass)
                {
                    sb.Append(comma).Append("LegacyArrayClass");
                    comma = ", ";
                }
                if (LegacyUnenumerableNamedProperties)
                {
                    sb.Append(comma).Append("LegacyUnenumerableNamedProperties");
                    comma = ", ";
                }
                if (OverrideBuiltins)
                {
                    sb.Append(comma).Append("OverrideBuiltins");
                    comma = ", ";
                }
                if (Unforgeable)
                {
                    sb.Append(comma).Append("Unforgeable");
                    comma = ", ";
                }
                if (NoInterfaceObject)
                {
                    sb.Append(comma).Append("NoInterfaceObject");
                }
                sb.AppendLine("]");
            }
            if (IsCallback)
            {
                sb.Append("callback ");
            }
            if (IsPartial)
            {
                sb.Append("partial ");
            }
            sb.Append("interface ");
            sb.Append(Name);
            if (Inherits.Any())
            {
                sb.Append(" : ").Append(string.Join(", ", Inherits));
            }
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
    }

    public class InterfaceNameCompare : IEqualityComparer<InterfaceType>
    {
        public bool Equals(InterfaceType x, InterfaceType y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(InterfaceType obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}