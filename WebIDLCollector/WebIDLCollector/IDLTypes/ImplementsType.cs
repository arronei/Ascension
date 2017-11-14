using System;
using System.Collections.Generic;

namespace WebIDLCollector.IDLTypes
{
    public class ImplementsType
    {
        public ImplementsType(string destinationInterface, string originatorInterface)
        {
            DestinationInterface = destinationInterface;
            OriginatorInterface = originatorInterface;
            SpecNames = new List<string>();
        }

        public Tuple<string, string> Key => new Tuple<string, string>(DestinationInterface, OriginatorInterface);

        public string DestinationInterface { get; }
        public string OriginatorInterface { get; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct(bool showMemberSpecName = false)
        {
            return DestinationInterface + " implements " + OriginatorInterface + (showMemberSpecName ? "; // " + string.Join(", ", SpecNames) : string.Empty);
        }

        public override bool Equals(object otherMember)
        {
            return (otherMember is ImplementsType member) &&
                   DestinationInterface.Equals(member.DestinationInterface, StringComparison.OrdinalIgnoreCase) &&
                   OriginatorInterface.Equals(member.OriginatorInterface, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return ((DestinationInterface.GetHashCode() << 5)
                    ^ OriginatorInterface.GetHashCode()) << 5;
        }
    }

    public class ImplementsCompare : IEqualityComparer<ImplementsType>
    {
        public bool Equals(ImplementsType x, ImplementsType y)
        {
            return y != null && (x != null && x.OriginatorInterface.Equals(y.OriginatorInterface, StringComparison.OrdinalIgnoreCase));
        }

        public int GetHashCode(ImplementsType obj)
        {
            return obj.OriginatorInterface.GetHashCode();
        }
    }
}