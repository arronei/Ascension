using System;
using System.Collections.Generic;
using System.Linq;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.Process
{
    public static class MergeProcessor
    {
        public static SpecData MergeSpecData(IEnumerable<SpecData> allSpecData, string name, bool keepPartials = false)
        {
            var finalInterfaceTypes = new SortedDictionary<string, InterfaceType>();
            var finalCallbackTypes = new SortedDictionary<string, CallbackType>();
            var finalDictionaryTypes = new SortedDictionary<string, DictionaryType>();
            var finalNamespaceTypes = new SortedDictionary<string, NamespaceType>();
            var finalImplementsTypes = new Dictionary<Tuple<string, string>, ImplementsType>();
            var finalTypeDefsTypes = new SortedDictionary<string, TypeDefType>();
            var finalEnumTypes = new SortedDictionary<string, EnumType>();

            //Consolidate all specs into merged interfaces, dictionaries, callbacks, etc..., merge partials
            foreach (var specData in allSpecData)
            {
                //Merge partials
                MergeInterfaces(specData, keepPartials, ref finalInterfaceTypes);

                MergeCallbacks(specData, ref finalCallbackTypes);

                MergeDictionaries(specData, keepPartials, ref finalDictionaryTypes);

                MergeNamespaces(specData, keepPartials, ref finalNamespaceTypes);

                MergeImplements(specData, ref finalImplementsTypes);

                MergeTypeDefs(specData, ref finalTypeDefsTypes);

                MergeEnumerations(specData, ref finalEnumTypes);
            }

            var fullSpecData = new SpecData
            {
                Name = name,
                Interfaces = finalInterfaceTypes.Values.ToList(),
                Callbacks = finalCallbackTypes.Values.ToList(),
                Dictionaries = finalDictionaryTypes.Values.ToList(),
                Namespaces = finalNamespaceTypes.Values.ToList(),
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

                currentCallback.TreatNonObjectAsNull = currentCallback.TreatNonObjectAsNull || callbackType.TreatNonObjectAsNull;

                currentCallback.SpecNames = currentCallback.SpecNames.Union(callbackType.SpecNames).OrderBy(a => a);

                finalCallbackTypes[callbackName] = currentCallback;
            }
        }

        private static void MergeDictionaries(SpecData data, bool keepPartials, ref SortedDictionary<string, DictionaryType> finalDictionaryTypes)
        {
            foreach (var dictionaryType in data.Dictionaries)
            {
                var dictionaryName = dictionaryType.Name;

                if (!finalDictionaryTypes.ContainsKey(dictionaryName))
                {
                    if (!keepPartials)
                    {
                        dictionaryType.IsPartial = false;
                    }

                    finalDictionaryTypes.Add(dictionaryName, dictionaryType);
                    continue;
                }

                var currentDictionary = finalDictionaryTypes[dictionaryName];
                if (!keepPartials)
                {
                    currentDictionary.IsPartial = false;
                }

                currentDictionary.Constructors = currentDictionary.Constructors.Union(dictionaryType.Constructors);
                currentDictionary.Exposed = currentDictionary.Exposed.Union(dictionaryType.Exposed).OrderBy(a => a);
                currentDictionary.Inherits = currentDictionary.Inherits.Union(dictionaryType.Inherits).OrderBy(a => a);
                currentDictionary.SpecNames = currentDictionary.SpecNames.Union(dictionaryType.SpecNames).OrderBy(a => a);

                var members = currentDictionary.Members.ToList();

                foreach (var member in dictionaryType.Members)
                {
                    if (!members.Contains(member, new DictionaryMemberCompare()))
                    {
                        members.Add(member);
                    }

                    var currentMember = members.Single(a => a.Equals(member));

                    currentMember.AllowShared = currentMember.AllowShared || member.AllowShared;
                    currentMember.Clamp = currentMember.Clamp || member.Clamp;
                    currentMember.EnforceRange = currentMember.EnforceRange || member.EnforceRange;

                    currentMember.SpecNames = currentMember.SpecNames.Union(member.SpecNames).OrderBy(a => a);

                    members.Remove(member);
                    members.Add(currentMember);
                }

                currentDictionary.Members = members;
                currentDictionary.Members = currentDictionary.Members.OrderBy(a => a.Name);

                finalDictionaryTypes[dictionaryName] = currentDictionary;
            }
        }

        private static void MergeNamespaces(SpecData data, bool keepPartials, ref SortedDictionary<string, NamespaceType> finalNamespaceTypes)
        {
            foreach (var namespaceType in data.Namespaces)
            {
                var namespaceName = namespaceType.Name;

                if (!finalNamespaceTypes.ContainsKey(namespaceName))
                {
                    if (!keepPartials)
                    {
                        namespaceType.IsPartial = false;
                    }
                    finalNamespaceTypes.Add(namespaceName, namespaceType);
                    continue;
                }

                var currentNamespace = finalNamespaceTypes[namespaceName];
                if (!keepPartials)
                {
                    currentNamespace.IsPartial = false;
                }

                currentNamespace.SecureContext = currentNamespace.SecureContext || namespaceType.SecureContext;
                currentNamespace.Exposed = currentNamespace.Exposed.Union(namespaceType.Exposed).OrderBy(a => a);
                currentNamespace.SpecNames = currentNamespace.SpecNames.Union(namespaceType.SpecNames).OrderBy(a => a);

                var members = currentNamespace.Members.ToList();

                foreach (var member in namespaceType.Members)
                {
                    if (!members.Contains(member, new NamespaceMemberCompare()))
                    {
                        members.Add(member);
                    }

                    var currentMember = members.Single(a => a.Equals(member));

                    currentMember.Exposed = currentMember.Exposed.Union(member.Exposed).OrderBy(a => a);
                    currentMember.SecureContext = currentMember.SecureContext || member.SecureContext;
                    currentMember.SpecNames = currentMember.SpecNames.Union(member.SpecNames).OrderBy(a => a);

                    members.Remove(member);
                    members.Add(currentMember);
                }
                currentNamespace.Members = members;
                currentNamespace.Members = currentNamespace.Members.OrderBy(a => a.Name);

                finalNamespaceTypes[namespaceName] = currentNamespace;
            }
        }

        private static void MergeInterfaces(SpecData data, bool keepPartials, ref SortedDictionary<string, InterfaceType> finalInterfaceTypes)
        {
            foreach (var interfaceType in data.Interfaces)
            {
                var interfaceName = interfaceType.Name;

                if (!finalInterfaceTypes.ContainsKey(interfaceName))
                {
                    if (!keepPartials)
                    {
                        interfaceType.IsPartial = false;
                    }
                    finalInterfaceTypes.Add(interfaceName, interfaceType);
                    continue;
                }

                var currentInterface = finalInterfaceTypes[interfaceName];
                if (!keepPartials)
                {
                    currentInterface.IsPartial = false;
                }

                currentInterface.IsCallback = currentInterface.IsCallback || interfaceType.IsCallback;
                currentInterface.ImplicitThis = currentInterface.ImplicitThis || interfaceType.ImplicitThis;
                currentInterface.HtmlConstructor = currentInterface.HtmlConstructor || interfaceType.HtmlConstructor;
                currentInterface.IsGlobal = currentInterface.IsGlobal || interfaceType.IsGlobal;
                currentInterface.IsPrimaryGlobal = currentInterface.IsPrimaryGlobal || interfaceType.IsPrimaryGlobal;
                currentInterface.LegacyArrayClass = currentInterface.LegacyArrayClass || interfaceType.LegacyArrayClass;
                currentInterface.LegacyUnenumerableNamedProperties = currentInterface.LegacyUnenumerableNamedProperties || interfaceType.LegacyUnenumerableNamedProperties;
                currentInterface.IsLegacyWindowAlias = currentInterface.IsLegacyWindowAlias || interfaceType.IsLegacyWindowAlias;
                currentInterface.NoInterfaceObject = currentInterface.NoInterfaceObject || interfaceType.NoInterfaceObject;
                currentInterface.OverrideBuiltins = currentInterface.OverrideBuiltins || interfaceType.OverrideBuiltins;
                currentInterface.SecureContext = currentInterface.SecureContext || interfaceType.SecureContext;
                currentInterface.Unforgeable = currentInterface.Unforgeable || interfaceType.Unforgeable;

                currentInterface.Exposed = currentInterface.Exposed.Union(interfaceType.Exposed).OrderBy(a => a);
                currentInterface.Globals = currentInterface.Globals.Union(interfaceType.Globals).OrderBy(a => a);
                currentInterface.PrimaryGlobals = currentInterface.PrimaryGlobals.Union(interfaceType.PrimaryGlobals).OrderBy(a => a);
                currentInterface.LegacyWindowAliases = currentInterface.LegacyWindowAliases.Union(interfaceType.LegacyWindowAliases).OrderBy(a => a);
                currentInterface.Constructors = currentInterface.Constructors.Union(interfaceType.Constructors);
                currentInterface.ExtendedBy = currentInterface.ExtendedBy.Union(interfaceType.ExtendedBy).OrderBy(a => a);
                currentInterface.Inherits = currentInterface.Inherits.Union(interfaceType.Inherits).OrderBy(a => a);
                currentInterface.NamedConstructors = currentInterface.NamedConstructors.Union(interfaceType.NamedConstructors);
                currentInterface.SpecNames = currentInterface.SpecNames.Union(interfaceType.SpecNames).OrderBy(a => a);

                var members = currentInterface.Members.ToList();

                foreach (var member in interfaceType.Members)
                {
                    if (!members.Contains(member, new MemberCompare()))
                    {
                        members.Add(member);
                    }

                    var currentMember = members.Single(a => a.Equals(member));

                    currentMember.AllowShared = currentMember.AllowShared || member.AllowShared;
                    currentMember.CeReactions = currentMember.CeReactions || member.CeReactions;
                    currentMember.Clamp = currentMember.Clamp || member.Clamp;
                    currentMember.Default = currentMember.Default || member.Default;
                    currentMember.EnforceRange = currentMember.EnforceRange || member.EnforceRange;
                    currentMember.Exposed = currentMember.Exposed.Union(member.Exposed).OrderBy(a => a);
                    currentMember.LenientSetter = currentMember.LenientSetter || member.LenientSetter;
                    currentMember.LenientThis = currentMember.LenientThis || member.LenientThis;
                    currentMember.NewObject = currentMember.NewObject || member.NewObject;
                    currentMember.PutForwards = currentMember.PutForwards ?? member.PutForwards;
                    currentMember.Replaceable = currentMember.Replaceable || member.Replaceable;
                    currentMember.SameObject = currentMember.SameObject || member.SameObject;
                    currentMember.SecureContext = currentMember.SecureContext || member.SecureContext;
                    currentMember.TreatNullAs = currentMember.TreatNullAs ?? member.TreatNullAs;
                    currentMember.TreatUndefinedAs = currentMember.TreatUndefinedAs ?? member.TreatUndefinedAs;
                    currentMember.Unforgeable = currentMember.Unforgeable || member.Unforgeable;
                    currentMember.Unscopable = currentMember.Unscopable || member.Unscopable;

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