using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WebIDLCollector.IDLTypes
{
    [Serializable]
    public class Property
    {
        public Property()
        {
            SpecNames = new List<string>();
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value.ToLowerInvariant(); }
        }

        public string OmName
        {
            get
            {
                var tempName = Name;
                if (tempName.Equals("float"))
                {
                    tempName = "css-" + Name;
                }
                return Regex.Replace(tempName.TrimStart('-'), @"-([a-z])", match => match.Groups[1].Value.ToUpperInvariant());
            }
        }

        public string WebkitOmName
        {
            get
            {
                if (!OmName.StartsWith("webkit"))
                {
                    return string.Empty;
                }
                return Regex.Replace(OmName, @"webkit", "Webkit");
            }
        }

        public string Value { get; set; }

        public string NewValue { get; set; }

        public string Initial { get; set; }

        public string NewInitialValue { get; set; }

        public string AppliesTo { get; set; }

        public string Inherited { get; set; }

        public string Percentages { get; set; }

        public string Media { get; set; }

        public string Animatable { get; set; }

        public string AnimationType { get; set; }

        public string ComputedValue { get; set; }

        public string NewComputedValue { get; set; }

        public string CanonicalOrder { get; set; }

        public string For { get; set; }

        public IEnumerable<string> SpecNames { get; set; }
    }
}