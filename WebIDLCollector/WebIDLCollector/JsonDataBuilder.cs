using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector
{
    public class JsonDataBuilder
    {
        private readonly bool _showSpecNames;
        private readonly SpecData _specData;

        public JsonDataBuilder(SpecData allCombinedSpecData)
        {
            _specData = allCombinedSpecData;
        }

        public void GenerateFile()
        {
            Console.WriteLine("Creating JsonObject");
            var jsonString = CreateJsonObject(_specData);
            using (var file = new StreamWriter("SpecMirror.js"))
            {
                file.WriteLine("{");
                file.WriteLine("\"browserVersion\": \"Specifications\", ");
                file.WriteLine("\"timestamp\": " + "\"" + DateTime.Now + "\",");
                file.WriteLine("\"types\": {");
                file.WriteLine(jsonString);
                file.WriteLine("}");
                file.WriteLine("}");
            }

            Console.WriteLine("Done writing.");
        }

        private object CreateJsonObject(SpecData _specData)
        {
            var finalList = new SortedDictionary<string, InterfaceType>(_specData.Interfaces.ToDictionary(a => a.Name, b => b), StringComparer.OrdinalIgnoreCase);

            foreach (var item in _specData.Implements)
            {
                var destinationItem = item.DestinationInterface;
                var originatorItem = item.OriginatorInterface;
                var implementsSpec = item.SpecNames;

                if (finalList.ContainsKey(destinationItem))
                {
                    if (finalList.ContainsKey(originatorItem))
                    {
                        var finalItem = finalList[destinationItem];
                        var originItem = finalList[originatorItem];
                        if (!finalList.ContainsKey(originatorItem))
                        {
                            continue;
                        }
                        var oItem = finalList[originatorItem];
                        if (!oItem.NoInterfaceObject)
                        {
                            finalItem.ExtendedBy =
                                _specData.Implements.Where(a => a.DestinationInterface == destinationItem)
                                    .Select(o => o.OriginatorInterface)
                                    .Distinct();
                        }
                        else
                        {
                            // Merge NoInterfaceObject members
                            var sortedMembers = new SortedDictionary<Tuple<string, string, IEnumerable<Argument>>, Member>(finalItem.Members.ToDictionary(a => a.Key, b => b));
                            foreach (var member in originItem.Members)
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
                            finalItem.Members = sortedMembers.Values;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Originator - " + originatorItem + " // " + implementsSpec);
                    }
                }
                else
                {
                    Console.WriteLine("Destination - " + destinationItem + " // " + implementsSpec);
                }
            }

            var noInterfaceObjects = _specData.Interfaces.Where(a => a.NoInterfaceObject);
            foreach (var noInterface in noInterfaceObjects)
            {
                finalList.Remove(noInterface.Name);
            }

            return finalList;
        }
    }
}