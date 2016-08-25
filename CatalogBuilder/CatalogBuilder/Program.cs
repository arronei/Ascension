using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CatalogBuilder.CatalogDataModel;
using CatalogBuilder.TypeMirrorDataModel;

namespace CatalogBuilder
{
    public class Program
    {
        public const string JsonFilePath = @"D:\GitHub\Ascension\TypeMirror\Data\";

        public static List<string> JsonFileNames { get; } = new List<string>
        {
            @".\TypeMirrorJsonFiles\SpecMirror.js"
        };

        //string[] jsonFileNames =
        //{
        //        @".\TypeMirrorJsonFiles\SpecMirror.js",
        //        @"D:\GitHub\Ascension\TypeMirror\Data\IE\IE14-Edge.js",
        //        @"D:\GitHub\Ascension\TypeMirror\Data\Chrome\Chrome54.js",
        //        @"D:\GitHub\Ascension\TypeMirror\Data\Firefox\Firefox51.js",
        //        @"D:\GitHub\Ascension\TypeMirror\Data\Safari\Safari10.js"
        //    };

        public static void Main(string[] args)
        {
            const string dataTableJsonOutputPath = @".\interfaceDataTable.json";
            const string apiViewOutputPath = @".\apiViewDataTable.json";
            const string vennOutputPath = @"D:\GitHub\MicrosoftEdge\APIComparisonData\vennData.json";
            const string specificationListPath = @"D:\GitHub\MicrosoftEdge\APIComparisonData\specifications.json";

            const string apiCatalogCsvDownloadPath = @"D:\GitHub\MicrosoftEdge\APIComparisonData\apiCatalog.csv";
            const string apiCatalogJsonDownloadPath = @"D:\GitHub\MicrosoftEdge\APIComparisonData\apiCatalog.json";

            CatalogObject catalogDataObject;

            PromptUser();

            if (JsonFileNames.Count == 5)
            {
                foreach (var jsonFileName in JsonFileNames)
                {
                    Console.Write("Deserialize " + jsonFileName + " ... ");
                    var typeMirrorObject = DeserializeJsonFile(jsonFileName);
                    if (typeMirrorObject == null)
                    {
                        continue;
                    }
                    Console.WriteLine("Done");
                    Console.Write("Add data for " + typeMirrorObject.BrowserVersion + " to ParityDataObject ...");

                    catalogDataObject = ProcessTypeMirrorObject(typeMirrorObject);

                    Console.WriteLine("Done");
                    Console.WriteLine();
                }
            }
            else
            {
                // this should never happen
            }
        }

        private static CatalogObject ProcessTypeMirrorObject(TypeMirrorObject typeMirrorObject)
        {
            var returnValue = new CatalogObject();
            var browserIdentifier = typeMirrorObject.BrowserVersion;

            return returnValue;
        }

        private static void PromptUser()
        {
            var browserList = new Dictionary<string, int[]>
                {
                    // "Browser Name", { min version, max version }
                    { "Edge", new[] { 12, 14 } },
                    { "Chrome", new[] { 10, 54 } },
                    { "Firefox", new[] { 23, 51 } },
                    { "Safari", new [] { 5, 10 } },
                };

            try
            {
                Console.WriteLine("Use defaults? (Y/N) Waiting 3 seconds...");
                var entry = Reader.ReadLine(3000);
                if (entry.Equals("N", StringComparison.InvariantCultureIgnoreCase))
                {
                    BuildJsonFilePathList(browserList);
                    Console.WriteLine();
                    return;
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Timeout expired. Using defaults...");
            }

            BuildJsonFilePathList(browserList, true);
            Console.WriteLine();
        }

        public static void BuildJsonFilePathList(Dictionary<string, int[]> browserList, bool useMax = false)
        {
            foreach (var item in browserList)
            {
                var browserName = item.Key;
                var minVersion = item.Value.Min();
                var maxVersion = item.Value.Max();
                var finalVersion = maxVersion;

                if (useMax)
                {
                    JsonFileNames.Add(string.Concat(JsonFilePath, $@"{browserName}\{browserName}{finalVersion}.js"));
                    continue;
                }
                while (true)
                {
                    Console.WriteLine($"Enter an {browserName} version between ({minVersion} and {maxVersion}):");
                    var browserVersion = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(browserVersion))
                    {
                        Console.WriteLine($"Using {browserName} default ({maxVersion}).");
                        break;
                    }
                    if ((int.TryParse(browserVersion, out finalVersion)) && (finalVersion >= minVersion && finalVersion <= maxVersion))
                    {
                        break;
                    }
                    //Console.WriteLine($"Enter a valid version number between {minVersion} and {maxVersion}.");
                }

                JsonFileNames.Add(string.Concat(JsonFilePath, $@"{browserName}\{browserName}{finalVersion}.js"));
            }
        }

        public static TypeMirrorObject DeserializeJsonFile(string fileString)
        {
            if (!File.Exists(fileString)) { return null; }
            using (var r = new StreamReader(fileString))
            {
                return JsonConvert.DeserializeObject<TypeMirrorObject>(r.ReadToEnd());
            }
        }
    }
}