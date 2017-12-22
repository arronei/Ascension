using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TypeSystem.Data.TypeMirror
{
    public class BrowserObject
    {
        [JsonProperty("BrowserVersion")]
        public string Name { get; set; }
        public string Timestamp { get; set; }
        [JsonProperty("Types")]
        public IReadOnlyDictionary<string, InterfaceObject> Interfaces { get; }

        public BrowserObject()
        {
            Interfaces = new Dictionary<string, InterfaceObject>();
        }

        public string ShortName => Name.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase) <= 0 ? Name : Name.Substring(0, Name.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase));

        public byte? Version => Name.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase) <= 0 ? (byte?) null : byte.Parse(Name.Substring(Name.LastIndexOf(" ", StringComparison.InvariantCultureIgnoreCase) + 1));
    }
}