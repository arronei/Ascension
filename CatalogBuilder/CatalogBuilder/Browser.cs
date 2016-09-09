namespace MS.Internal
{
    public class Browser
    {
        /// <summary>The vendor name that produces the browser</summary>
        public string BrowserVendor { get; set; }

        public string BrowserFullName => string.Concat(BrowserVendor, " ", BrowserName);

        /// <summary>A simplified name not containing the vendor</summary>
        public string BrowserName { get; set; }

        /// <summary></summary>
        public byte MinimumSupportedVersion { get; set; }

        /// <summary></summary>
        public byte MaximumSupportedVersion { get; set; }
    }
}