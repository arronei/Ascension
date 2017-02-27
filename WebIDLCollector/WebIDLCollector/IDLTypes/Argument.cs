using System.Collections.Generic;
using System.Text;

namespace WebIDLCollector.IDLTypes
{
    public class Argument
    {
        public Argument(string name)
        {
            Name = name;
        }

        private string Name { get; }
        public string Type { get; set; }
        public bool Optional { get; set; }
        public string ExtendedAttribute { get; set; }
        public bool Clamp { get; set; }
        public bool EnforceRange { get; set; }
        public string TreatNullAs { get; set; }
        public bool Ellipsis { get; set; }
        public string Value { get; set; }

        private string Reconstruct()
        {
            var sb = new StringBuilder();
            var comma = string.Empty;

            if (Clamp ||
                EnforceRange ||
                !string.IsNullOrWhiteSpace(TreatNullAs))
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
                if (!string.IsNullOrWhiteSpace(TreatNullAs))
                {
                    sb.Append(comma).Append("TreatNullAs=").Append(TreatNullAs);
                }

                sb.Append("] ");
            }

            if (Optional)
            {
                sb.Append("optional ");
            }
            sb.Append(Type);
            if (Ellipsis)
            {
                sb.Append("...");
            }
            sb.Append(" ");
            sb.Append(Name);
            if (!string.IsNullOrWhiteSpace(Value))
            {
                sb.Append(" = ");
                sb.Append(Value);
            }

            return sb.ToString().Trim();
        }

        public static string ReconstructArgs(IEnumerable<Argument> argTypes)
        {
            var sb = new StringBuilder();

            var comma = string.Empty;
            foreach (var argument in argTypes)
            {
                sb.Append(comma).Append(argument.Reconstruct());
                comma = ", ";
            }

            return sb.ToString();
        }
    }

    public class ArgumentTypeCompare : IEqualityComparer<Argument>
    {
        public bool Equals(Argument x, Argument y)
        {
            return x.Type.Equals(y.Type) &&
                    x.Optional.Equals(y.Optional) &&
                    x.Clamp.Equals(y.Clamp) &&
                    x.EnforceRange.Equals(y.EnforceRange) &&
                    (x.TreatNullAs == null || x.TreatNullAs.Equals(y.TreatNullAs)) &&
                    x.Ellipsis.Equals(y.Ellipsis) &&
                    (x.Value == null || x.Value.Equals(y.Value));
        }

        public int GetHashCode(Argument obj)
        {
            return ((((((((((((obj.Type.GetHashCode() << 5)
                                ^ obj.Optional.GetHashCode()) << 5)
                              ^ obj.Clamp.GetHashCode()) << 5)
                            ^ obj.EnforceRange.GetHashCode()) << 5)
                          ^ obj.TreatNullAs.GetHashCode()) << 5)
                        ^ obj.Ellipsis.GetHashCode()) << 5)
                      ^ obj.Value.GetHashCode()) << 5;
        }
    }
}