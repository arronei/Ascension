using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using WebIDLCollector.GetData;

namespace WebIDLCollector.Process
{
    public static partial class ProcessSpecs
    {
        public static void ProcessRespec(IEnumerable<IElement> respecItems, string selector, SpecData specificationData)
        {
            var cleanString = CleanString(GenerateRespecIdls(respecItems, selector));

            specificationData.Callbacks.AddRange(DataCollectors.GetAllCallbacks(cleanString, specificationData));
            specificationData.Dictionaries.AddRange(DataCollectors.GetAllDictionaries(cleanString, specificationData));
            specificationData.Enumerations.AddRange(DataCollectors.GetAllEnums(cleanString, specificationData));
            specificationData.Implements.AddRange(DataCollectors.GetAllImplements(cleanString, specificationData));
            specificationData.Interfaces.AddRange(DataCollectors.GetAllInterfaces(cleanString, specificationData));
            specificationData.Namespaces.AddRange(DataCollectors.GetAllNamespaces(cleanString, specificationData));
            specificationData.TypeDefs.AddRange(DataCollectors.GetAllTypeDefs(cleanString, specificationData));
        }

        private static string GenerateRespecIdls(IEnumerable<IElement> respecItems, string selector)
        {
            var allIdls = string.Empty;
            foreach (var definition in respecItems)
            {
                var idlItem = definition.GetAttribute("title");
                if (idlItem.Contains("interface") || idlItem.Contains("dictionary") || idlItem.Contains("namespace") || idlItem.Contains("enum"))
                {
                    idlItem += " {";
                    var members = definition.QuerySelectorAll(selector + " > dt");
                    var endChar = idlItem.Contains("enum") ? "," : ";\r\n";

                    var memberData = string.Empty;
                    foreach (var member in members)
                    {
                        var content = member.TextContent.Trim();
                        if (content.Contains("("))
                        {
                            var dd = member.NextElementSibling;
                            var paramItems = dd?.QuerySelectorAll("dl.parameters dt");
                            if (paramItems?.Length > 0)
                            {
                                var parameters = paramItems.Aggregate(string.Empty, (current, parameter) => current + (parameter.TextContent + ", ")).Trim().TrimEnd(',');
                                if (content.Contains("()"))
                                {
                                    content = content.Replace("()", "(" + parameters + ")");
                                }
                            }
                        }

                        memberData += content + endChar;
                    }

                    memberData = idlItem.Contains("enum") ? memberData.TrimEnd(',') : memberData;
                    idlItem += memberData + "}";
                }
                else if (idlItem.Contains("callback"))
                {
                    var members = definition.QuerySelectorAll(selector + " > dt");

                    var memberData = string.Empty;
                    var comma = string.Empty;
                    foreach (var member in members)
                    {
                        memberData += comma + member.TextContent.Trim();
                        comma = ", ";
                    }
                    idlItem += " (" + memberData + ");\r\n";
                }
                else
                {
                    idlItem += ";";
                }
                allIdls += idlItem;
            }
            return allIdls;
        }
    }
}