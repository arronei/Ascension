using System;
using System.Collections.Generic;
using System.Linq;
using TypeSystem.Data.Core;

namespace TypeSystem.Data.SpecRef
{
    public class SpecRef : BaseSerializarionJson<SortedDictionary<string, SpecRefObject>>
    {
        private static SortedDictionary<string, string> _specRefExtraList;

        public SpecRef(SortedDictionary<string, string> specRefExtraList)
        {
            _specRefExtraList = specRefExtraList;
        }

        public SortedDictionary<string, string> ProcessData(SortedDictionary<string, SpecRefObject> specRefData)
        {
            Console.WriteLine("Process data for SpecRef ...");

            var specRefShortNameTitle = new SortedDictionary<string, string>();
            foreach (var specRefType in specRefData)
            {
                var title = specRefType.Value.Title;
                if (string.IsNullOrWhiteSpace(title))
                {
                    var alias = specRefType.Value.AliasOf;
                    while (!string.IsNullOrWhiteSpace(alias))
                    {
                        if (specRefData.ContainsKey(alias))
                        {
                            var latestRefType = specRefData[alias];
                            if (!string.IsNullOrWhiteSpace(latestRefType.AliasOf))
                            {
                                alias = latestRefType.AliasOf;
                                continue;
                            }
                            title = latestRefType.Title;
                        }
                        break;
                    }
                }

                if (!specRefShortNameTitle.ContainsKey(specRefType.Key.ToLowerInvariant()))
                {
                    specRefShortNameTitle.Add(specRefType.Key.ToLowerInvariant(), title);
                }
            }

            Console.WriteLine("Processing for SpecRef Complete.");
            Console.WriteLine();

            return new SortedDictionary<string, string>(specRefShortNameTitle.Union(_specRefExtraList).ToLookup(k => k.Key, v => v.Value).ToDictionary(k => k.Key, v => v.First()));
        }

        public new SortedDictionary<string, SpecRefObject> DeserializeJsonData(string url)
        {
            Console.WriteLine($"Retrieve SpecRef data ({url})...");

            var returnValue = new SortedDictionary<string, SpecRefObject>();

            var specRefDictionary = new BaseSerializarionJson<IDictionary<string, dynamic>>().DeserializeJsonData(new Uri(url));
            foreach (var specRefItem in specRefDictionary)
            {
                var shortName = specRefItem.Key;

                SpecRefObject specRef;
                if (specRefItem.Value is string)
                {
                    specRef = new SpecRefObject
                    {
                        Data = specRefItem.Value
                    };
                }
                else
                {
                    specRef = new SpecRefObject
                    {
                        AliasOf = specRefItem.Value["aliasOf"],
                        Date = specRefItem.Value["date"],
                        Href = specRefItem.Value["href"],
                        EdDraft = specRefItem.Value["edDraft"],
                        Title = specRefItem.Value["title"]
                    };
                }
                returnValue.Add(shortName, specRef);
            }

            return returnValue;
        }
    }
}