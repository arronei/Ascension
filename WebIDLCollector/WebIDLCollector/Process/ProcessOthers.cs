using System;
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
            //var cleanString = Regex.Replace(Regex.Replace(Regex.Replace(fileItems.Replace("&lt;", "<").Replace("&gt;", ">"), @"/\*.*?\*/", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline), @"\s*//.*$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline), @"\s+", " ", RegexOptions.Singleline | RegexOptions.Multiline).Trim();

            var cleanString = CleanString(fileItems);

            specificationData.Callbacks.AddRange(DataCollectors.GetAllCallbacks(cleanString, specificationData));
            specificationData.Dictionaries.AddRange(DataCollectors.GetAllDictionaries(cleanString, specificationData));
            specificationData.Enumerations.AddRange(DataCollectors.GetAllEnums(cleanString, specificationData));
            specificationData.Implements.AddRange(DataCollectors.GetAllImplements(cleanString, specificationData));
            specificationData.Interfaces.AddRange(DataCollectors.GetAllInterfaces(cleanString, specificationData));
            specificationData.TypeDefs.AddRange(DataCollectors.GetAllTypeDefs(cleanString, specificationData));
        }

        public static void ProcessIdl(IEnumerable<IElement> idlItems, SpecData specificationData)
        {
            foreach(var cleanString in idlItems.Select(item => CleanString(item.TextContent)))
            {
                //var cleanString = Regex.Replace(Regex.Replace(Regex.Replace(fileItems.Replace("&lt;", "<").Replace("&gt;", ">"), @"/\*.*?\*/", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline), @"\s*//.*$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline), @"\s+", " ", RegexOptions.Singleline | RegexOptions.Multiline).Trim();

                specificationData.Callbacks.AddRange(DataCollectors.GetAllCallbacks(cleanString, specificationData));
                specificationData.Dictionaries.AddRange(DataCollectors.GetAllDictionaries(cleanString, specificationData));
                specificationData.Enumerations.AddRange(DataCollectors.GetAllEnums(cleanString, specificationData));
                specificationData.Implements.AddRange(DataCollectors.GetAllImplements(cleanString, specificationData));
                specificationData.Interfaces.AddRange(DataCollectors.GetAllInterfaces(cleanString, specificationData));
                specificationData.TypeDefs.AddRange(DataCollectors.GetAllTypeDefs(cleanString, specificationData));
            }
        }

        public static string CleanString(string value)
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