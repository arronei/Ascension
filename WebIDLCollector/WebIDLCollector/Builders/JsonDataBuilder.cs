using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebIDLCollector.IDLTypes;
using WebIDLCollector.TypeMirrorTypes;

namespace WebIDLCollector.Builders
{
    public class JsonDataBuilder
    {
        private readonly SpecData _specData;

        public JsonDataBuilder(SpecData allCombinedSpecData)
        {
            _specData = allCombinedSpecData;
        }

        public void GenerateFile()
        {
            var typeMirrorFile = "typeMirror/SpecMirror.js";
            Console.WriteLine("Creating JsonObject");
            var jsonString = CreateJsonObject();
            if (File.Exists(typeMirrorFile))
            {
                File.Delete(typeMirrorFile);
            }
            using (var file = new StreamWriter(typeMirrorFile))
            {
                file.WriteLine("{");
                file.WriteLine("\"browserVersion\": \"Specifications\",");
                file.WriteLine("\"timestamp\": " + "\"" + DateTime.Now + "\",");
                file.WriteLine("\"types\": {");
                file.WriteLine(jsonString);
                file.WriteLine("}");
                file.WriteLine("}");
            }
        }

        private string CreateJsonObject()
        {
            var sb = new StringBuilder();
            var finalList = GenerateFinalInterfaceList();
            var commaPlaceHolder = string.Empty;

            foreach (var interfaceType in finalList.Values)
            {
                var tmType = new TypeMirrorType
                {
                    TypeName = interfaceType.Name,
                    BaseType = string.Join(", ", interfaceType.Inherits),
                    Confidence = 4,
                    DerivedTypes = interfaceType.ExtendedBy.ToList(),
                    Properties = new List<TypeMirrorProperty>(),
                    SpecNames = interfaceType.SpecNames
                };

                var mem = interfaceType.Members.ToList();

                if (!interfaceType.NoInterfaceObject)
                {
                    var constructor = new TypeMirrorProperty
                    {
                        Name = "constructor",
                        SpecNames = interfaceType.SpecNames,
                        Confidence = 4,
                        IsConfigurable = true,
                        IsWritable = true,
                    };

                    tmType.Properties.Add(constructor);

                    BuilderHelpers.AddNameLengthProto(tmType, mem);
                }

                foreach (var tmProperty in mem)
                {
                    if (BuilderHelpers.AddIterable(tmProperty, tmType, mem)) { continue; }
                    if (BuilderHelpers.AddMaplike(tmProperty, tmType, mem)) { continue; }
                    if (BuilderHelpers.AddSerializer(tmProperty, tmType, mem)) { continue; }
                    if (BuilderHelpers.AddSetlike(tmProperty, tmType, mem)) { continue; }
                    if (BuilderHelpers.AddStringifier(tmProperty, tmType, mem)) { continue; }
                    if (string.IsNullOrWhiteSpace(tmProperty.Name)) { continue; }
                    var item = new TypeMirrorProperty
                    {
                        Name = tmProperty.Name,
                        Type = tmProperty.Type,
                        Confidence = 4,
                        HasGet = tmProperty.HasGet,
                        HasSet = tmProperty.HasSet,
                        IsConfigurable = true,
                        IsEnumerable = true,
                        IsWritable = true,
                        SpecNames = tmProperty.SpecNames
                    };

                    tmType.Properties.Add(item);
                }

                sb.Append(commaPlaceHolder);
                commaPlaceHolder = ",\r\n";
                sb.Append(tmType);
            }

            return sb.ToString();
        }

        private Dictionary<string, InterfaceType> GenerateFinalInterfaceList()
        {
            var finalList = _specData.Interfaces.ToDictionary(a => a.Name, b => b);

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
                            var sortedMembers = finalItem.Members.ToDictionary(a => a.Key, b => b);
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
                            finalItem.Members.OrderBy(a => a.Name);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Originator - " + originatorItem + " // " + string.Join(", ", implementsSpec));
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Destination - " + destinationItem + " // " + string.Join(", ", implementsSpec));
                    Console.ForegroundColor = ConsoleColor.Gray;
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