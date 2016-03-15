using System.Collections.Generic;

namespace WebIDLCollector.IDLTypes
{
    public class Argument
    {
        public Argument(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public string Type { get; set; }
        public bool In { get; set; }
        public bool Optional { get; set; }
        public string ExtendedAttribute { get; set; }
        public bool Clamp { get; set; }
        public bool EnforceRange { get; set; }
        public string TreatNullAs { get; set; }
        public string TreatUndefinedAs { get; set; }
        public bool Ellipsis { get; set; }
        public string Value { get; set; }

        public string Reconstruct
        {
            get
            {
                return
                    (
                        (!string.IsNullOrWhiteSpace(ExtendedAttribute) ? "[" + ExtendedAttribute + "] " : string.Empty) +
                        (Optional ? "optional " : string.Empty) +
                        Type + " " +
                        (Ellipsis ? "... " : string.Empty) +
                        Name +
                        (!string.IsNullOrWhiteSpace(Value) ? " = " + Value : string.Empty)
                    ).TrimEnd();
            }
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
                   x.TreatNullAs.Equals(y.TreatNullAs) &&
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