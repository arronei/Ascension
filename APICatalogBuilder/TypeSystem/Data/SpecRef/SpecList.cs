using System;
using Newtonsoft.Json;

namespace TypeSystem.Data.SpecRef
{
    [Serializable]
    public class SpecList
    {
        [JsonProperty("shortName")]
        public string ShortName { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}