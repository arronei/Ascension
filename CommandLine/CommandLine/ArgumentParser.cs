using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace MS.Internal.CommandLine
{
    /// <summary>Processes command line arguments</summary>
    public class ArgumentParser
    {
        #region Private members
        /// <summary>Identify names arguments</summary>
        private static readonly Regex IdentifyArgument = new Regex(@"^(--|-|/)", RegexOptions.Compiled);

        /// <summary></summary>
        private static readonly Regex Spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.Compiled);

        /// <summary></summary>
        private static readonly Regex Spliter2 = new Regex(@"^-{1,2}|^/|=", RegexOptions.Compiled);

        /// <summary></summary>
        private static readonly Regex RemoveMatchingQuotes = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.Compiled);
        #endregion

        #region Public accessors
        /// <summary>Named arguments</summary>
        public Dictionary<string, string> Named { get; }

        /// <summary>Unnamed arguments</summary>
        public Collection<string> Unnamed { get; }
        #endregion

        /// <summary>Processesses the command line arguments and stores the arguments in two collections; named and unnamed.</summary>
        /// <param name="passedInArguments">The arguments to be processed.</param>
        public ArgumentParser(IEnumerable<string> passedInArguments) : this(passedInArguments, true)
        {
        }

        /// <summary>Processesses the command line arguments and stores the arguments in two collections; named and unnamed.</summary>
        /// <param name="passedInArguments">The arguments to be processed.</param>
        /// <param name="caseSensitive">Flag determining if arguments are to be handled in a case-sensitive manner. (default: true)</param>
        public ArgumentParser(IEnumerable<string> passedInArguments, bool caseSensitive)
        {
            if (null == passedInArguments) { return; }

            string parameter = null;
            Named = new Dictionary<string, string>((caseSensitive ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase));
            Unnamed = new Collection<string>();

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples:
            // -param1 value1 --param2 /param3:"Test-:-work"
            //   /param4=happy -param5 '--=nice=--'
            foreach (var parts in passedInArguments.Select(text => IdentifyArgument.IsMatch(text) ? Spliter.Split(text, 3) : Spliter2.Split(text, 3)))
            {
                switch (parts.Length)
                {
                    // Found a value (for the last parameter found (space separator))
                    case 1:
                        if (null != parameter)
                        {
                            if (!string.IsNullOrEmpty(parts[0]))
                            {
                                parts[0] = RemoveMatchingQuotes.Replace(parts[0], "$1");
                            }

                            if (!Named.ContainsKey(parameter))
                            {
                                Named.Add(parameter, parts[0]);
                            }
                        }
                        else    //Not null or empty
                        {
                            Unnamed.Add(parts[0]);
                        }
                        parameter = null;
                        // else Error: no parameter waiting for a value (skipped)
                        break;
                    // Found just a parameter
                    case 2:
                    case 3:
                        // The last parameter is still waiting.
                        // With no value, set it to true.
                        if (!string.IsNullOrEmpty(parameter))
                        {
                            if (!Named.ContainsKey(parameter))
                            {
                                Named.Add(parameter, "true");
                            }
                        }
                        parameter = parts[1];
                        if (2 == parts.Length) { break; }

                        // case 3: Parameter with enclosed value
                        // Remove possible enclosing characters (",')
                        parts[2] = RemoveMatchingQuotes.Replace(parts[2], "$1");
                        if (!Named.ContainsKey(parameter))
                        {
                            Named.Add(parameter, parts[2]);
                        }

                        parameter = null;
                        break;
                }
            }
            // In case a parameter is still waiting
            if (null == parameter) { return; }
            if (!Named.ContainsKey(parameter))
            {
                Named.Add(parameter, "true");
            }
        }

        /// <summary>Retrieve a parameter value if it exists (overriding C# indexer property)</summary>
        /// <param name="key">The key to look up</param>
        /// <returns>The value for the argument specified</returns>
        public string this[string key] => (Named[key]);
    }
}