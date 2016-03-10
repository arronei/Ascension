using System;
using System.Collections.Generic;
using System.Linq;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.Process
{
    public static class MergeProcessor
    {
        public static SpecData MergeSpecData(IEnumerable<SpecData> allSpecData)
        {
            var finalInterfaceTypes = new SortedDictionary<string, InterfaceType>();
            var finalCallbackTypes = new SortedDictionary<string, CallbackType>();
            var finalDictionaryTypes = new SortedDictionary<string, DictionaryType>();
            var finalImplementsTypes = new Dictionary<Tuple<string, string>, ImplementsType>();
            var finalTypeDefsTypes = new SortedDictionary<string, TypeDefType>();
            var finalEnumTypes = new SortedDictionary<string, EnumType>();

            //Consolidate all specs into merged interfaces, dictionaries, callbacks, etc..., merge partials
            foreach (var specData in allSpecData)
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

                var members = currentInterface.Members.ToList();

                foreach (var member in interfaceType.Members)
                {
                    if (!members.Contains(member))
                    {
                        members.Add(member);
                    }

                    var currentMember = members.Single(a => a.Equals(member));

                    currentMember.SpecNames = currentMember.SpecNames.Union(member.SpecNames).OrderBy(a => a);

                    members.Remove(member);
                    members.Add(currentMember);
                }
                currentInterface.Members = members;
                currentInterface.Members = currentInterface.Members.OrderBy(a => a.Name);

                finalInterfaceTypes[interfaceName] = currentInterface;
            }
        }
    }
}