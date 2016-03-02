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
            var cleanString = Regex.Replace(fileItems.Replace("&lt;", "<").Replace("&gt;", ">").Trim(), @"/\*.*?\*/", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            specificationData.Callbacks.AddRange(DataCollectors.GetAllCallbacks(cleanString, specificationData));
            specificationData.Dictionaries.AddRange(DataCollectors.GetAllDictionaries(cleanString, specificationData));
            specificationData.Enumerations.AddRange(DataCollectors.GetAllEnums(cleanString, specificationData));
            specificationData.Implements.AddRange(DataCollectors.GetAllImplements(cleanString, specificationData));
            specificationData.Interfaces.AddRange(DataCollectors.GetAllInterfaces(cleanString, specificationData));
            specificationData.TypeDefs.AddRange(DataCollectors.GetAllTypeDefs(cleanString, specificationData));
        }

        public static void ProcessIdl(IEnumerable<IElement> idlItems, SpecData specificationData)
        {
            foreach (var cleanString in idlItems.Select(item => item.TextContent).Select(s => s.Replace("&lt;", "<").Replace("&gt;", ">").Trim()).Select(cleanString => Regex.Replace(cleanString, @"/\*.*?\*/", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline)))
            {
                specificationData.Callbacks.AddRange(DataCollectors.GetAllCallbacks(cleanString, specificationData));
                specificationData.Dictionaries.AddRange(DataCollectors.GetAllDictionaries(cleanString, specificationData));
                specificationData.Enumerations.AddRange(DataCollectors.GetAllEnums(cleanString, specificationData));
                specificationData.Implements.AddRange(DataCollectors.GetAllImplements(cleanString, specificationData));
                specificationData.Interfaces.AddRange(DataCollectors.GetAllInterfaces(cleanString, specificationData));
                specificationData.TypeDefs.AddRange(DataCollectors.GetAllTypeDefs(cleanString, specificationData));
            }
        }
    }
}