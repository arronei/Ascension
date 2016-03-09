using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AngleSharp;
using Newtonsoft.Json;
using WebIDLCollector.Builders;
using WebIDLCollector.IDLTypes;
using WebIDLCollector.Process;

namespace WebIDLCollector
{
    [Serializable]
    public class SpecRef
    {
        public SpecRef()
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

    public class Program
    {
        public static void Main(string[] args)
        {
            const string webidlLocation = "webidl";
            if (Directory.Exists(webidlLocation))
            {
                Console.WriteLine("Deleting old files...");
                Directory.Delete(webidlLocation, true);
            }

            //Console.WriteLine("Retrieve SpecRef data...");
            //var request = WebRequest.Create("https://specref.herokuapp.com/bibrefs");
            //var speRefData = new SortedDictionary<string, SpecRef>();
            //using (var response = request.GetResponse())
            //{
            //    using (var responseStream = response.GetResponseStream())
            //    {
            //        if (responseStream == null)
            //        {
            //            Console.ForegroundColor = ConsoleColor.Red;
            //            Console.WriteLine("Unable to get SpecRef data.");
            //            Console.ForegroundColor = ConsoleColor.Gray;
            //            Console.ReadKey();
            //            return;
            //        }
            //        using (var stream = new StreamReader(responseStream))
            //        {
            //            var specRefSerializer = new JsonSerializer();
            //            var specRefDictionary = (IDictionary<string, dynamic>)specRefSerializer.Deserialize(stream, typeof(IDictionary<string, dynamic>));
            //            foreach (var item in specRefDictionary)
            //            {
            //                var shortName = item.Key;

            //                SpecRef specRef;
            //                if (item.Value is string)
            //                {
            //                    specRef = new SpecRef
            //                    {
            //                        Data = item.Value
            //                    };
            //                }
            //                else
            //                {
            //                    specRef = new SpecRef
            //                    {
            //                        AliasOf = item.Value["aliasOf"],
            //                        Date = item.Value["date"],
            //                        Href = item.Value["href"],
            //                        Title = item.Value["title"]
            //                    };
            //                }

            //                speRefData.Add(shortName, specRef);
            //            }
            //        }
            //    }
            //}

            ProcessJsonFile("specData.json");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Generation Complete!");
            Console.ReadKey();
        }

        private static void ProcessJsonFile(string jsonFile)
        {
            if ((string.IsNullOrWhiteSpace(jsonFile)) || (!File.Exists(jsonFile)))
            {
                return;
            }

            var allSpecData = new List<SpecData>();

            using (var file = File.OpenText(jsonFile))
            {
                var serializer = new JsonSerializer();
                var specDataList = ((IEnumerable<SpecData>)serializer.Deserialize(file, typeof(IEnumerable<SpecData>))).ToList();

                if (!specDataList.Any())
                {
                    return;
                }

                // Debug single item
                //specDataList = new List<SpecData>
                //{
                //    new SpecData
                //    {
                //        Name = "mediastream-recording",
                //        Url = "https://w3c.github.io/mediacapture-record/MediaRecorder.html",
                //        Identification = new List<SpecIdentification>()
                //        {
                //            new SpecIdentification
                //            {
                //                Selector = "dl.idl",
                //                Type = "respec"
                //            }
                //        }
                //    }
                //};

                foreach (var specData in specDataList)
                {
                    if (!ProcessSpec(specData))
                    {
                        continue;
                    }
                    var webIdl = new WebIdlBuilder(specData);
                    webIdl.GenerateFile();

                    allSpecData.Add(specData);
                }
            }

            var mergedSpecData = MergeSpecData(allSpecData);
            var allWebIdl = new WebIdlBuilder(mergedSpecData, true);
            allWebIdl.GenerateFile();

            var jsonDataBuilder = new JsonDataBuilder(mergedSpecData);
            jsonDataBuilder.GenerateFile();
        }

        private static SpecData MergeSpecData(IEnumerable<SpecData> allSpecData)
        {
            var finalInterfaceTypes = new SortedDictionary<string, InterfaceType>();
            var finalCallbackTypes = new SortedDictionary<string, CallbackType>();
            var finalDictionaryTypes = new SortedDictionary<string, DictionaryType>();
            var finalImplementsTypes = new Dictionary<Tuple<string, string>, ImplementsType>();
            var finalTypeDefsTypes = new SortedDictionary<string, TypeDefType>();
            var finalEnumTypes = new SortedDictionary<string, EnumType>();

            //Consolidate all specs into merged interfaces, dictionaries, callbacks, etc..., merge partials
            foreach (var specData in allSpecData)
            {
                //Merge partials
                MergeInterfaces(specData, ref finalInterfaceTypes);

                MergeCallbacks(specData, ref finalCallbackTypes);

                MergeDictionaries(specData, ref finalDictionaryTypes);

                MergeImplements(specData, ref finalImplementsTypes);

                MergeTypeDefs(specData, ref finalTypeDefsTypes);

                MergeEnumerations(specData, ref finalEnumTypes);
            }

            var fullSpecData = new SpecData
            {
                Name = "Specifications",
                Interfaces = finalInterfaceTypes.Values.ToList(),
                Callbacks = finalCallbackTypes.Values.ToList(),
                Dictionaries = finalDictionaryTypes.Values.ToList(),
                Implements = finalImplementsTypes.Values.ToList(),
                TypeDefs = finalTypeDefsTypes.Values.ToList(),
                Enumerations = finalEnumTypes.Values.ToList()
            };

            return fullSpecData;
        }

        private static void MergeEnumerations(SpecData data, ref SortedDictionary<string, EnumType> finalEnumTypes)
        {
            foreach (var enumType in data.Enumerations)
            {
                var enumTypeName = enumType.Name;

                if (!finalEnumTypes.ContainsKey(enumTypeName))
                {
                    finalEnumTypes.Add(enumTypeName, enumType);
                    continue;
                }

                var currentEnumeration = finalEnumTypes[enumTypeName];
                currentEnumeration.SpecNames = currentEnumeration.SpecNames.Union(enumType.SpecNames).OrderBy(a => a);

                finalEnumTypes[enumTypeName] = currentEnumeration;
            }
        }

        private static void MergeTypeDefs(SpecData data, ref SortedDictionary<string, TypeDefType> finalTypeDefsTypes)
        {
            foreach (var typeDefType in data.TypeDefs)
            {
                var typeDefName = typeDefType.Name;

                if (!finalTypeDefsTypes.ContainsKey(typeDefName))
                {
                    finalTypeDefsTypes.Add(typeDefName, typeDefType);
                    continue;
                }

                var currentTypeDef = finalTypeDefsTypes[typeDefName];
                currentTypeDef.SpecNames = currentTypeDef.SpecNames.Union(typeDefType.SpecNames).OrderBy(a => a);

                finalTypeDefsTypes[typeDefName] = currentTypeDef;
            }
        }

        private static void MergeImplements(SpecData data, ref Dictionary<Tuple<string, string>, ImplementsType> finalImplementsTypes)
        {
            foreach (var implementsType in data.Implements)
            {
                var implementsKey = implementsType.Key;

                if (!finalImplementsTypes.ContainsKey(implementsKey))
                {
                    finalImplementsTypes.Add(implementsKey, implementsType);
                    continue;
                }

                var currentImplements = finalImplementsTypes[implementsKey];
                currentImplements.SpecNames = currentImplements.SpecNames.Union(implementsType.SpecNames).OrderBy(a => a);

                finalImplementsTypes[implementsKey] = currentImplements;
            }
        }

        private static void MergeCallbacks(SpecData data, ref SortedDictionary<string, CallbackType> finalCallbackTypes)
        {
            foreach (var callbackType in data.Callbacks)
            {
                var callbackName = callbackType.Name;

                if (!finalCallbackTypes.ContainsKey(callbackName))
                {
                    finalCallbackTypes.Add(callbackName, callbackType);
                    continue;
                }

                var currentCallback = finalCallbackTypes[callbackName];
                currentCallback.SpecNames = currentCallback.SpecNames.Union(callbackType.SpecNames).OrderBy(a => a);

                finalCallbackTypes[callbackName] = currentCallback;
            }
        }

        private static void MergeDictionaries(SpecData data, ref SortedDictionary<string, DictionaryType> finalDictionaryTypes)
        {
            foreach (var dictionaryType in data.Dictionaries)
            {
                var dictionaryName = dictionaryType.Name;

                if (!finalDictionaryTypes.ContainsKey(dictionaryName))
                {
                    dictionaryType.IsPartial = false;
                    finalDictionaryTypes.Add(dictionaryName, dictionaryType);
                    continue;
                }

                var currentDictionary = finalDictionaryTypes[dictionaryName];
                currentDictionary.IsPartial = false;
                var constructors = currentDictionary.Constructors as IList<string> ?? currentDictionary.Constructors.ToList();
                currentDictionary.Constructors = constructors.Union(constructors);
                currentDictionary.Exposed = currentDictionary.Exposed.Union(currentDictionary.Exposed).OrderBy(a => a);
                currentDictionary.Inherits = currentDictionary.Inherits.Union(dictionaryType.Inherits).OrderBy(a => a);
                currentDictionary.SpecNames = currentDictionary.SpecNames.Union(dictionaryType.SpecNames).OrderBy(a => a);

                var sortedMembers = new SortedDictionary<string, DictionaryMember>(currentDictionary.Members.ToDictionary(a => a.Name, b => b));
                foreach (var member in dictionaryType.Members)
                {
                    var memberName = member.Name;

                    if (!sortedMembers.ContainsKey(memberName))
                    {
                        sortedMembers.Add(memberName, member);
                    }

                    var currentMemeber = sortedMembers[memberName];
                    currentMemeber.SpecNames = currentMemeber.SpecNames.Union(member.SpecNames).OrderBy(a => a);

                    sortedMembers[memberName] = currentMemeber;
                }

                currentDictionary.Members = sortedMembers.Values;

                finalDictionaryTypes[dictionaryName] = currentDictionary;
            }
        }

        private static void MergeInterfaces(SpecData data, ref SortedDictionary<string, InterfaceType> finalInterfaceTypes)
        {
            foreach (var interfaceType in data.Interfaces)
            {
                var interfaceName = interfaceType.Name;

                if (!finalInterfaceTypes.ContainsKey(interfaceName))
                {
                    interfaceType.IsPartial = false;
                    finalInterfaceTypes.Add(interfaceName, interfaceType);
                    continue;
                }

                var currentInterface = finalInterfaceTypes[interfaceName];
                currentInterface.IsPartial = false;
                currentInterface.Constructors = currentInterface.Constructors.Union(interfaceType.Constructors);
                currentInterface.ExtendedBy = currentInterface.ExtendedBy.Union(interfaceType.ExtendedBy).OrderBy(a => a);
                currentInterface.Inherits = currentInterface.Inherits.Union(interfaceType.Inherits).OrderBy(a => a);
                currentInterface.NamedConstructors = currentInterface.NamedConstructors.Union(interfaceType.NamedConstructors);
                currentInterface.SpecNames = currentInterface.SpecNames.Union(interfaceType.SpecNames).OrderBy(a => a);

                var members = currentInterface.Members.ToList();

                foreach (var member in interfaceType.Members)
                {
                    if (!members.Contains(member))
                    {
                        members.Add(member);
                    }

                    var currentMember = members.Single(a => a.Equals(member));

                    currentMember.SpecNames = currentMember.SpecNames.Union(member.SpecNames).OrderBy(a => a);

                    members.Remove(member);
                    members.Add(currentMember);
                }
                currentInterface.Members = members;
                currentInterface.Members = currentInterface.Members.OrderBy(a => a.Name);

                finalInterfaceTypes[interfaceName] = currentInterface;
            }
        }

        private static bool ProcessSpec(SpecData specData)
        {
            Console.WriteLine("Processing (" + specData.Url + ")");

            //var specShortName = specData.Name;  //Lookup spec URL from respec data file
            var specUrl = specData.Url;
            var specFile = specData.File;

            var config = Configuration.Default.WithDefaultLoader();
            var document = BrowsingContext.New(config).OpenAsync(specUrl).Result;

            if (!specData.Identification.Any())
            {
                //Determine Bikeshed
                var meta = document.QuerySelector("meta[name=generator]");
                if (meta != null && meta.GetAttribute("content").StartsWith("bikeshed", StringComparison.InvariantCultureIgnoreCase))
                {
                    var bikeshedIdentificationList = new List<SpecIdentification>();
                    var idlIndex = document.QuerySelector("#idl-index");
                    if (idlIndex != null && idlIndex.HasChildNodes) // determine IDL Index
                    {
                        bikeshedIdentificationList.Add(new SpecIdentification
                        {
                            Selector = "#idl-index + pre.idl",
                            Type = "idl"
                        });
                    }
                    var propDef = document.QuerySelector("table.propdef");
                    if (propDef != null && propDef.HasChildNodes) // determine propdef table
                    {
                        bikeshedIdentificationList.Add(new SpecIdentification
                        {
                            Selector = "table.propdef",
                            Type = "bikeshed"
                        });
                    }
                    if (!bikeshedIdentificationList.Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No standards bikeshed sections found");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    specData.Identification = bikeshedIdentificationList;//Change this to .Add()
                }
                //Determine respec
                //else if (){}

            }
            if (!specData.Identification.Any() && string.IsNullOrWhiteSpace(specFile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to determine what to parse");
                Console.ForegroundColor = ConsoleColor.Gray;
                return false;
            }

            foreach (var specIdentification in specData.Identification)
            {
                switch (specIdentification.Type)
                {
                    case "css":
                        var cssItems = document.QuerySelectorAll(specIdentification.Selector);
                        ProcessSpecs.ProcessCss(cssItems, specData);
                        break;
                    case "svgcss":
                        var svgCssItems = document.QuerySelector(specIdentification.Selector);
                        ProcessSpecs.ProcessSvGCss(svgCssItems, specData);
                        break;
                    case "bikeshed":
                        var bikeshedItems = document.QuerySelectorAll(specIdentification.Selector);
                        ProcessSpecs.ProcessBikeshed(bikeshedItems, specData);
                        break;
                    case "respec":
                        var respecItems = document.QuerySelectorAll(specIdentification.Selector);
                        ProcessSpecs.ProcessRespec(respecItems, specIdentification.Selector, specData);
                        break;
                    case "idl":
                        var idlItems = document.QuerySelectorAll(specIdentification.Selector);
                        ProcessSpecs.ProcessIdl(idlItems, specData);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (!string.IsNullOrWhiteSpace(specFile))
            {
                var fileItems = File.ReadAllText("data/" + specFile);
                ProcessSpecs.ProcessFile(fileItems, specData);
            }

            if (!specData.Callbacks.Any() &&
                !specData.Dictionaries.Any() &&
                !specData.Enumerations.Any() &&
                !specData.Implements.Any() &&
                !specData.Interfaces.Any() &&
                !specData.TypeDefs.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.Write("Dictionaries: " + specData.Dictionaries.Count + ", ");
            Console.Write("Enumerations: " + specData.Enumerations.Count + ", ");
            Console.Write("Implements: " + specData.Implements.Count + ", ");
            Console.Write("Interfaces: " + specData.Interfaces.Count + ", ");
            Console.Write("TypeDefs: " + specData.TypeDefs.Count + ", ");
            Console.WriteLine("Callbacks: " + specData.Callbacks.Count);
            if (Console.ForegroundColor == ConsoleColor.Red)
            {
                Console.ReadKey();
            }

            Console.ForegroundColor = ConsoleColor.Gray;

            return true;
        }
    }
}