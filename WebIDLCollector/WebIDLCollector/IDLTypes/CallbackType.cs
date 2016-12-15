using System.Collections.Generic;
using System.Text;

namespace WebIDLCollector.IDLTypes
{
    public class CallbackType
    {
        public CallbackType(string name)
        {
            Name = name;
            ArgTypes = new List<Argument>();
            SpecNames = new List<string>();
        }

        public string Name { get; }
        public string Type { private get; set; }
        public string Args { get; set; }
        public IEnumerable<Argument> ArgTypes { private get; set; }
        public string ExtendedAttribute { get; set; }
        public bool TreatNonObjectAsNull { get; set; }
        public bool TreatNonCallableAsNull { get; set; }
        public IEnumerable<string> SpecNames { get; set; }

        public string Reconstruct
        {
            get
            {
                return (TreatNonObjectAsNull ? "[TreatNonObjectAsNull] " : string.Empty) + "callback " + Name + " = " + Type + " (" + ReconstructArgs(ArgTypes) + "); // " + string.Join(", ", SpecNames);
            }
        }

        private static string ReconstructArgs(IEnumerable<Argument> argTypes)
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
}