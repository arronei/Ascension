using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using Newtonsoft.Json;
using WebIDLCollector.Builders;
using WebIDLCollector.Process;

namespace WebIDLCollector
{
    public static class Program
    {
        public static void Main()
        {
            const string webidlLocation = "webidl";
            if (Directory.Exists(webidlLocation))
            {
                Console.WriteLine("Deleting old files...");
                Directory.Delete(webidlLocation, true);
            }

            ProcessJsonFile("specData.json");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Generation Complete!");
            Console.ReadLine();
        }

        private static void ProcessJsonFile(string jsonFile)
        {
            if (string.IsNullOrWhiteSpace(jsonFile) || !File.Exists(jsonFile))
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

                //Debug single item
                //specDataList = new List<SpecData>
                //{
                //    new SpecData
                //    {
                //        Name = "navigatorcores",
                //        Url = "https://wiki.whatwg.org/wiki/Navigator_HW_Concurrency",
                //        File = "navigatorcores.webidl"
                //    }
                //};

                allSpecData.AddRange(specDataList.Where(ProcessSpec));

                var specNames = allSpecData.Select(a => a.Name).Distinct();
                foreach (var webIdl in from specName in specNames let specSpecData = allSpecData.Where(a => a.Name.Equals(specName)) select MergeProcessor.MergeSpecData(specSpecData, specName, true) into mergedSpecSpecData select new WebIdlBuilder(mergedSpecSpecData))
                {
                    webIdl.GenerateFile();
                }
            }

            var mergedSpecData = MergeProcessor.MergeSpecData(allSpecData, "Specifications");
            var allWebIdl = new WebIdlBuilder(mergedSpecData, true);
            allWebIdl.GenerateFile();

            var jsonDataBuilder = new JsonDataBuilder(mergedSpecData);
            jsonDataBuilder.GenerateFile();
        }

        private static bool ProcessSpec(SpecData specData)
        {
            Console.WriteLine("Processing (" + specData.Url + ")");

            //var specShortName = specData.Name;
            //var specTitle = specData.Title;
            var specUrl = specData.Url;
            var specFile = specData.File;

            var config = Configuration.Default.WithDefaultLoader();
            var document = BrowsingContext.New(config).OpenAsync(specUrl).Result;

            specData.Identification = AutoDetectSpecDataIdentification(specData, document);
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
                !specData.Namespaces.Any() &&
                !specData.Enumerations.Any() &&
                !specData.Implements.Any() &&
                !specData.Interfaces.Any() &&
                !specData.TypeDefs.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.Write("Enumerations: " + specData.Enumerations.Count + ", ");
            Console.Write("Implements: " + specData.Implements.Count + ", ");
            Console.Write("Dictionaries: " + specData.Dictionaries.Count + ", ");
            Console.Write("Namespaces: " + specData.Namespaces.Count + ", ");
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

        private static List<SpecIdentification> AutoDetectSpecDataIdentification(SpecData specData, IDocument document)
        {
            if (specData.Identification.Any())
            {
                return specData.Identification;
            }

            var specIdentification = new List<SpecIdentification>();
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
                else
                {
                    var preIdlBikeshed = document.QuerySelector("pre.idl");
                    if (preIdlBikeshed != null && preIdlBikeshed.HasChildNodes)
                    {
                        bikeshedIdentificationList.Add(new SpecIdentification
                        {
                            Selector = "pre.idl",
                            Type = "idl"
                        });
                    }
                }
                var bikeshedPropDef = document.QuerySelector("table.propdef");
                if (bikeshedPropDef != null && bikeshedPropDef.HasChildNodes) // determine propdef table
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
                    Console.WriteLine("No common bikeshed sections found");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    specIdentification.AddRange(bikeshedIdentificationList);
                }
            }
            //Determine respec
            else if (Regex.IsMatch(document.DocumentElement.OuterHtml, @"\brespec\b", RegexOptions.IgnoreCase))
            {
                var respecIdentificationList = new List<SpecIdentification>();
                var dlIdl = document.QuerySelector("dl.idl");
                if (dlIdl != null)
                {
                    respecIdentificationList.Add(new SpecIdentification
                    {
                        Selector = "dl.idl",
                        Type = "respec"
                    });
                }
                var preIdlRespec = document.QuerySelector("pre.idl");
                if (preIdlRespec != null && preIdlRespec.HasChildNodes)
                {
                    respecIdentificationList.Add(new SpecIdentification
                    {
                        Selector = "pre.idl",
                        Type = "idl"
                    });
                }
                if (!respecIdentificationList.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No common respec sections found");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    specIdentification.AddRange(respecIdentificationList);
                }
            }

            if (specIdentification.Any())
            {
                return specIdentification;
            }

            //Fallback case
            var fallbackIdentificationList = new List<SpecIdentification>();
            var preIdl = document.QuerySelector("pre.idl");
            if (preIdl != null && preIdl.HasChildNodes)
            {
                fallbackIdentificationList.Add(new SpecIdentification
                {
                    Selector = "pre.idl",
                    Type = "idl"
                });
            }
            var propDef = document.QuerySelector("table.propdef");
            if (propDef != null && propDef.HasChildNodes)
            {
                fallbackIdentificationList.Add(new SpecIdentification
                {
                    Selector = "table.propdef",
                    Type = "bikeshed"
                });
            }
            if (!fallbackIdentificationList.Any() && string.IsNullOrWhiteSpace(specData.File))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No common sections to parse");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            specIdentification.AddRange(fallbackIdentificationList);

            return specIdentification;
        }
    }
}