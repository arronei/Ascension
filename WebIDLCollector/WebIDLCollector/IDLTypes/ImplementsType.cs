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

        public Tuple<string, string> Key
        {
            get { return new Tuple<string, string>(DestinationInterface, OriginatorInterface); }
        }

        public string DestinationInterface { get; }
        public string OriginatorInterface { get; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct
        {
            get
            {
                return DestinationInterface + " implements " + OriginatorInterface + "; // " + string.Join(", ", SpecNames);
            }
        }

        public override bool Equals(object otherMember)
        {
            var member = otherMember as ImplementsType;
            return (member != null) &&
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
            return x.OriginatorInterface.Equals(y.OriginatorInterface, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(ImplementsType obj)
        {
            return obj.OriginatorInterface.GetHashCode();
        }
    }
}