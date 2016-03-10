using System;
using System.Collections.Generic;

namespace WebIDLCollector.SpecRef
{
    [Serializable]
    public class SpecRefType
    {
        public SpecRefType()
        {
            Authors = new List<string>();
            Versions = new List<string>();
            ObsoletedBy = new List<string>();
        }

        public string Href { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Id { get; set; }
        public string AliasOf { get; set; }
        public string Publisher { get; set; }
        public List<string> Authors { get; }
        public List<string> Versions { get; }
        public List<string> ObsoletedBy { get; }
        public string Data { get; set; }
    }
}