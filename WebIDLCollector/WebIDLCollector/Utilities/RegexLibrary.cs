using System.Text.RegularExpressions;

namespace WebIDLCollector.Utilities
{
    public static class RegexLibrary
    {
        public static Regex GroupingCleaner { get; } = new Regex(@"[\(\)\s]+", RegexOptions.Compiled);

        public static Regex TypeCleaner { get; } = new Regex(@"\s+\?", RegexOptions.Compiled);

        public static Regex OldTypeCleaner { get; } = new Regex(@"[a-z]*::", RegexOptions.Compiled);

        public static Regex ParenCleaner { get; } = new Regex(@"\(\)", RegexOptions.Compiled);

        public static Regex WhitespaceCleaner { get; } = new Regex(@"\s*", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

        public static Regex PropertyCleaner { get; } = new Regex(@"[^-a-z]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}