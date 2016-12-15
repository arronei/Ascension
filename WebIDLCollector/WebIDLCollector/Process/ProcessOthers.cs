using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using WebIDLCollector.GetData;

namespace WebIDLCollector.Process
{
    public static partial class ProcessSpecs
    {
        public static void ProcessFile(string fileItems, SpecData specificationData)
        {
            var cleanString = CleanString(fileItems);

            specificationData.Callbacks.AddRange(DataCollectors.GetAllCallbacks(cleanString, specificationData));
            specificationData.Dictionaries.AddRange(DataCollectors.GetAllDictionaries(cleanString, specificationData));
            specificationData.Enumerations.AddRange(DataCollectors.GetAllEnums(cleanString, specificationData));
            specificationData.Implements.AddRange(DataCollectors.GetAllImplements(cleanString, specificationData));
            specificationData.Interfaces.AddRange(DataCollectors.GetAllInterfaces(cleanString, specificationData));
            specificationData.Namespaces.AddRange(DataCollectors.GetAllNamespaces(cleanString, specificationData));
            specificationData.TypeDefs.AddRange(DataCollectors.GetAllTypeDefs(cleanString, specificationData));
        }

        public static void ProcessIdl(IEnumerable<IElement> idlItems, SpecData specificationData)
        {
            foreach (var cleanString in idlItems.Select(item => CleanString(item.TextContent)))
            {
                specificationData.Callbacks.AddRange(DataCollectors.GetAllCallbacks(cleanString, specificationData));
                specificationData.Dictionaries.AddRange(DataCollectors.GetAllDictionaries(cleanString, specificationData));
                specificationData.Enumerations.AddRange(DataCollectors.GetAllEnums(cleanString, specificationData));
                specificationData.Implements.AddRange(DataCollectors.GetAllImplements(cleanString, specificationData));
                specificationData.Interfaces.AddRange(DataCollectors.GetAllInterfaces(cleanString, specificationData));
                specificationData.Namespaces.AddRange(DataCollectors.GetAllNamespaces(cleanString, specificationData));
                specificationData.TypeDefs.AddRange(DataCollectors.GetAllTypeDefs(cleanString, specificationData));
            }
        }

        private static string CleanString(string value)
        {
            value = Regex.Replace(value, @"\s*&lt;\s*", "<");
            value = Regex.Replace(value, @"\s*&gt;\s*", ">");
            value = Regex.Replace(value, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);
            value = Regex.Replace(value, @"\s*//.*$", string.Empty, RegexOptions.Multiline);
            value = Regex.Replace(value, @"\s+", " ", RegexOptions.Multiline);

            return value;
        }
    }
}