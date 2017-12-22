using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Generator.SpecificationList;
using Generator.Venn;
using TypeSystem.Data.ApiCatalog;
using TypeSystem.Data.Browser;
using TypeSystem.Data.Core;
using TypeSystem.Data.SpecRef;
using TypeSystem.Data.TypeMirror;

namespace MS.Internal
{
    public static class ApiCatalogBuilder
    {
        private static readonly string BrowserReleaseFilePath = (ConfigurationManager.AppSettings["fullBrowserVersionListFilePath"] ?? @".\DataFiles\BrowserRelease.json");

        private static readonly string SpecDataFilePath = ConfigurationManager.AppSettings["specDataFile"] ?? @".\DataFiles\SpecMirror.js";

        private static readonly string[] BrowserOrder = (ConfigurationManager.AppSettings["browserOrder"] ?? "Edge,Chrome,Firefox,Safari").Split(',');

        private static readonly string[] BrowserReleaseList = (ConfigurationManager.AppSettings["browserReleaseList"] ?? string.Empty).Split(',');

        private static readonly string SpecRefUrl = ConfigurationManager.AppSettings["specRefUrl"] ?? "https://specref.herokuapp.com/bibrefs";

        private static readonly string SpecRefExtra = ConfigurationManager.AppSettings["specRefExtra"] ?? @".\DataFiles\SpecRefMissingList.json";

        private static BrowsersCollection _fullBrowserReleaseList;

        private static string[] _fileList;

        private static CatalogObject CatalogDataObject { get; set; }

        public static void Main()
        {
            PromptUser();

            //Prep CatalogDataObject
            CatalogDataObject = new CatalogObject(ProcessSpecRef());

            ProcessFileList();

            var specListObject = new SpecificationiListGenerator();
            var specListToWrite = specListObject.GenerateSpecificDataObject(CatalogDataObject);
            specListObject.Write(@".\output\specifications.json", specListToWrite);

            var venn = new VennGenerator();
            venn.WriteAllSpectifications(CatalogDataObject, @".\output\vennData\");
            //var t = venn.GenerateSpecificDataObject(CatalogDataObject);
            //venn.Write(@".\output\vennData\vennData.json", t);

            //var browsers = CatalogDataObject.Interfaces.Values.SelectMany(a => a.SupportedBrowsers).Distinct().OrderBySequence(BrowserOrder).ToList();
        }

        private static SortedDictionary<string, string> ProcessSpecRef()
        {
            var specRefExtraList = new BaseSerializarionJson<SortedDictionary<string, string>>().DeserializeJsonData(SpecRefExtra);

            var specRefObject = new SpecRef(specRefExtraList);
            var specRefData = specRefObject.DeserializeJsonData(SpecRefUrl);
            if (!specRefData.Any())
            {
                // throw an error
            }
            return specRefObject.ProcessData(specRefData);
        }

        private static void ProcessFileList()
        {
            if (!_fileList.Any())
            {
                return;
            }
            foreach (var filePath in _fileList)
            {
                var typeMirrorObject = new TypeMirror();
                var browserTypeMirrorData = typeMirrorObject.DeserializeJsonDataFile(filePath);
                if (browserTypeMirrorData == null)
                {
                    continue;
                }

                typeMirrorObject.ProcessData(CatalogDataObject, browserTypeMirrorData, _fullBrowserReleaseList);
            }
        }

        private static void PromptUser()
        {
            try
            {
                Console.WriteLine("Use defaults? (Y/N) Waiting 3 seconds...");
                var entry = Reader.ReadLine(3000);
                if (entry.Equals("N", StringComparison.InvariantCultureIgnoreCase))
                {
                    GetJsonFilePathList();
                    Console.WriteLine();
                    return;
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Timeout expired. Using defaults...");
            }

            GetJsonFilePathList(true);
            Console.WriteLine();
        }

        private static void GetJsonFilePathList(bool useDefaults = false)
        {
            var returnValue = new List<string>();
            _fullBrowserReleaseList = new Browser().DeserializeJsonDataFile(BrowserReleaseFilePath);
            _fullBrowserReleaseList.BrowserOrder = BrowserOrder;

            // Always add spec data first
            returnValue.Add(SpecDataFilePath);

            if (useDefaults)
            {
                returnValue.AddRange(_fullBrowserReleaseList.GetAllBrowserFilePaths());
            }
            else
            {

                var browserShortNames = _fullBrowserReleaseList.GetAllBrowserShortNames(BrowserReleaseList);
                foreach (var browserShortName in browserShortNames)
                {
                    var browserName = _fullBrowserReleaseList.GetLatestItem(browserShortName).Name;
                    var versionRange = _fullBrowserReleaseList.MinMaxByBrowser(browserShortName);
                    var finalVersion = versionRange.Max;
                    while (true)
                    {
                        Console.WriteLine($"Enter an {browserName} version between ({versionRange.Min} and {versionRange.Max}):");
                        var browserVersion = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(browserVersion))
                        {
                            Console.WriteLine($"Using {browserName} default ({versionRange.Max}).");
                            break;
                        }
                        if (byte.TryParse(browserVersion, out finalVersion) && finalVersion >= versionRange.Min && finalVersion <= versionRange.Max)
                        {
                            break;
                        }
                        Console.WriteLine($"Enter a valid version number between {versionRange.Min} and {versionRange.Max} (inclusive).");
                    }

                    returnValue.Add(_fullBrowserReleaseList.GetItem(browserShortName, finalVersion).GetFilePath());
                }
            }

            _fileList = returnValue.ToArray();
        }
    }
}