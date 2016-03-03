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
    public class SpecData
    {
        public SpecData()
        {
            Implements = new List<ImplementsType>();
            Interfaces = new List<InterfaceType>();
            Dictionaries = new List<DictionaryType>();
            Enumerations = new List<EnumType>();
            TypeDefs = new List<TypeDefType>();
            Callbacks = new List<CallbackType>();
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public string File { get; set; }

        public IEnumerable<SpecIdentification> Identification;

        public List<ImplementsType> Implements { get; set; }

        public List<InterfaceType> Interfaces { get; set; }

        public List<DictionaryType> Dictionaries { get; set; }

        public List<EnumType> Enumerations { get; set; }

        public List<TypeDefType> TypeDefs { get; set; }

        public List<CallbackType> Callbacks { get; set; }
    }

    public class SpecIdentification
    {
        public string Selector { get; set; }
        public string Type { get; set; }//Change this to an enum
    }

    public class Program
    {
        public static SortedDictionary<string, SpecData> AllSpecData = new SortedDictionary<string, SpecData>();

        public static void Main(string[] args)
        {
            ProcessJsonFile("specData.json");
            Console.ReadKey();
        }

        private static void ProcessJsonFile(string jsonFile)
        {
            if ((string.IsNullOrWhiteSpace(jsonFile)) || (!File.Exists(jsonFile)))
            {
                return;
            }

            using (var file = File.OpenText(jsonFile))
            {
                var serializer = new JsonSerializer();
                var specDataList = ((IEnumerable<SpecData>)serializer.Deserialize(file, typeof(IEnumerable<SpecData>))).ToList();

                if (!specDataList.Any())
                {
                    return;
                }

                foreach (var specData in specDataList)
                {
                    ProcessSpec(specData);
                    var webIdl = new WebIdlBuilder(specData);
                    webIdl.GenerateFile();

                    AllSpecData.Add(specData.Name, specData);
                }
            }

            var mergedSpecData = MergeSpecData();
            var allWebIdl = new WebIdlBuilder(mergedSpecData, true);
            allWebIdl.GenerateFile();

            var jsonDataBuilder = new JsonDataBuilder(mergedSpecData);
            jsonDataBuilder.GenerateFile();
        }

        private static SpecData MergeSpecData()
        {
            var finalInterfaceTypes = new SortedDictionary<string, InterfaceType>();
            var finalCallbackTypes = new SortedDictionary<string, CallbackType>();
            var finalDictionaryTypes = new SortedDictionary<string, DictionaryType>();
            var finalImplementsTypes = new Dictionary<Tuple<string, string>, ImplementsType>();
            var finalTypeDefsTypes = new SortedDictionary<string, TypeDefType>();
            var finalEnumTypes = new SortedDictionary<string, EnumType>();

            //Consolidate all specs into merged interfaces, dictionaries, callbacks, etc..., merge partials
            foreach (var specData in AllSpecData.Values)
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

                var sortedMembers = new SortedDictionary<Tuple<string, string, IEnumerable<Argument>>, Member>(currentInterface.Members.ToDictionary(a => a.Key, b => b));

                foreach (var member in interfaceType.Members)
                {
                    var memberKey = member.Key;

                    if (!sortedMembers.ContainsKey(memberKey))
                    {
                        sortedMembers.Add(memberKey, member);
                    }

                    var currentMember = sortedMembers[memberKey];

                    currentMember.SpecNames = currentMember.SpecNames.Union(member.SpecNames).OrderBy(a => a);

                    sortedMembers[memberKey] = currentMember;
                }
                currentInterface.Members = sortedMembers.Values;

                finalInterfaceTypes[interfaceName] = currentInterface;
            }
        }

        private static void ProcessSpec(SpecData specData)
        {
            Console.WriteLine("Processing (" + specData.Url + ")");

            //var specShortName = specData.Name;  //Lookup spec URL from respec data file
            var specUrl = specData.Url;
            var specFile = specData.File;

            foreach (var specIdentification in specData.Identification)
            {
                if (specIdentification.Type != "file")
                {
                    var config = Configuration.Default.WithDefaultLoader();
                    var document = BrowsingContext.New(config).OpenAsync(specUrl).Result;
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
                else
                {
                    var fileItems = File.ReadAllText("data/" + specFile);
                    ProcessSpecs.ProcessFile(fileItems, specData);
                }
            }

            Console.Write("Dictionaries: " + specData.Dictionaries.Count + ", ");
            Console.Write("Enumerations: " + specData.Enumerations.Count + ", ");
            Console.Write("Implements: " + specData.Implements.Count + ", ");
            Console.Write("Interfaces: " + specData.Interfaces.Count + ", ");
            Console.Write("TypeDefs: " + specData.TypeDefs.Count + ", ");
            Console.WriteLine("Callbacks: " + specData.Callbacks.Count);
        }
    }
}