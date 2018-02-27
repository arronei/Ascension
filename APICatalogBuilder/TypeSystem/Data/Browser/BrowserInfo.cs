using System;

namespace TypeSystem.Data.Browser
{
    [Serializable]
    public class BrowserInfo
    {
        private const string Path = @"F:\GitHub\Ascension\TypeMirror\Data\";

        private string _shortName;

        public BrowserInfo(string name, byte version, DateTime? releaseDate, string release)
        {
            Name = name;
            Version = version;
            ReleaseDate = releaseDate;
            Release = release;
        }

        public string Name { get; }

        public string ShortName
        {
            get => string.IsNullOrWhiteSpace(_shortName) ? Name.Substring(Name.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase) + 1) : _shortName;
            set => _shortName = value;
        }

        public byte Version { get; }

        public DateTime? ReleaseDate { get; }

        public string Release { get; }

        public string GetFullName()
        {
            return $"{Name}" + (!Name.Equals("Specifications")? $" {Version}" : string.Empty) + (string.IsNullOrWhiteSpace(Release) || !Name.Contains("Edge") ? string.Empty : $" ({Release})");
        }

        public string GetFilePath()
        {
            return Path + ShortName + @"\" + ShortName + Version + ".js";
        }
    }
}