using Newtonsoft.Json;

namespace MS.Internal
{
    public class Browser
    {
        /// <summary>The vendor name that produces the browser</summary>
        public string Vendor { get; set; }

        public string FullName => $"{Vendor} {Name}";

        /// <summary>A simplified name not containing the vendor</summary>
        public string Name { get; set; }

        /// <summary></summary>
        [JsonProperty("MinVersion")]
        public byte MinimumSupportedVersion { get; set; }

        /// <summary></summary>
        [JsonProperty("MaxVersion")]
        public byte MaximumSupportedVersion { get; set; }
    }
}