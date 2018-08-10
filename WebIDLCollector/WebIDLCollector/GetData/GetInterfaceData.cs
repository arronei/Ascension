using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebIDLCollector.Builders;
using WebIDLCollector.IDLTypes;
using WebIDLCollector.Utilities;

namespace WebIDLCollector.GetData
{
    public static partial class DataCollectors
    {
        private static readonly Regex InterfaceParser = new Regex(@"(\[(?<extended>[^\]]+)\]\s*)?
        (((?<partial>partial)|(?<callback>callback))\s+)?(/\*[^\*]+\*/\s*)?interface\s+(/\*[^\*]+\*/\s*)?
        ((?<mixin>mixin)\s+(/\*[^\*]+\*/\s*)?)?(?<name>\w+)(?:\s*:\s*(?<inherits>[\w,\s]+))?\s*\{\s*(?<members>([^\}]*?\{.*?\};.*?|[^\}]+))?\s*\};?", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

        private static readonly Regex InterfaceExtendedParser = new Regex(@"((?<constructor>constructor(\s*\((?<args>.+?)?\))?)(,|$)|
        (?<namedconstructor>namedconstructor\s*=\s*[^\)\s,\]]+(\s*\((?<ncargs>.+)?\))?)(,|$)|
        (exposed(\s*=\s*(?<exposed>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<global>global(\s*=\s*(?<globals>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<htmlconstructor>htmlconstructor)(,|$)|
        (?<implicitthis>implicitthis)(,|$)|
        [^y](?<arrayclass>arrayclass)(,|$)|
        (?<legacyarrayclass>legacyarrayclass)(,|$)|
        (?<legacyunenumerablenamedproperties>legacyunenumerablenamedproperties)(,|$)|
        (?<legacywindowalias>legacywindowalias(\s*=\s*(?<legacywindowaliases>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<nointerfaceobject>nointerfaceobject)(,|$)|
        (?<overridebuiltins>overridebuiltins)(,|$)|
        (?<primaryglobal>primaryglobal(\s*=\s*(?<primaryglobals>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<securecontext>securecontext)(,|$)|
        (?<unforgeable>unforgeable)(,|$))+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);


        private const string extended = @"(\[(?<extended>[^\]]+)]\s*)?";

        private const string type = @"(\s*(?<uniontype>\(.+?\)+\??)\s*|(\s*\[(?<typeextended>[^\]]+)]\s*|\s+)((?<recordtype>.+>+\??)\s*|(dom\s*::\s*)?(?<type>[^\s:\(\)<>]+?)\s+))";

        private const string specialOperations = @"(((?<getter>getter)|(?<setter>setter)|(?<creator>creator)|(?<deleter>deleter)|(?<legacycaller>legacycaller))(?(?![\[\(])\s+)){1,5}" + type + @"(?<item>\w+)?\s*(?<function>\((?<args>.*)\))|";
        private const string constant = @"(?<const>const)" + type + @"(?<item>\w+?)\s*=\s*(?<value>.+?)|";
        private const string serializer = @"(?<serializer>serializer)(" + type + @"((?< item >\w+?)\s*)?(?<function>\((?<args>.*)\))|\s*=\s*(?<bracket>((?<curly>\{)|(?<square>\[)))?\s*(?<value>[^;}\]\)]*)\s*(?(curly)\}|(?(square)\])))?|";
        private const string stringifier = @"(?<stringifier>stringifier)((\s+(?<readonly>readonly))?\s+(?<attribute>attribute)" + type + @"((?<required>required)\s+)?(?<item>\w+)|" + type + @"(?<item>\w+)?(?<function>\((?<args>.*)\)))?|";



        private static readonly Regex IndividualInterfaceMember = new Regex(@"^\s*(\[(?<extended>[^\]]+)]\s*)?(
        ((((?<getter>getter)|(?<setter>setter)|(?<creator>creator)|(?<deleter>deleter)|(?<legacycaller>legacycaller))\s+){1,5}\s*)(dom::)?(?<type>.+)\s+(?<item>[^(\s]+)?\s*(?<function>\((?<args>.*)\))|
        (?<const>const)\s+(dom::)?(?<type>.+)\s+(?<item>.+?)\s*=\s*(?<value>.+?)|
        (?<serializer>serializer)(\s+(dom::)?(?<type>.+)\s+((?<item>[^\(\s]+)\s*)?(?<function>\((?<args>.*)\))|\s*=\s*(?<bracket>[\{\[])?\s*(?<value>[^\}\]]*)\s*[\}\]]?)?|
        (?<stringifier>stringifier)((\s+((?<readonly>readonly)\s+)?(?<attribute>attribute)\s+(?<type>.+)(\s+(?<required>required))?(\s+(?<item>[^;]+)))|(\s+(?<type>.+))(\s+(?<item>.+))?(?<function>\((?<args>.*)\)))?$|
        (?<static>static)\s+(((?<readonly>readonly)\s+)?(?<attribute>attribute)\s+(?<type>.+)\s+((?<required>required)|(?<item>.+))|(?<type>.+?)\s*((?<item>[^\(\s]+)\s*)?(?<function>\((?<args>.*)\)))|
        ((?<iterable>iterable)|(?<legacyiterable>legacyiterable))\s*<(?<type>.+)>|
        (?<readonly>readonly)\s+((?<attribute>attribute)\s+(?<type>.+)\s+((?<required>required)|(?<item>.+))|(?<maplike>maplike)\s*<(?<type>.+)>|(?<setlike>setlike)\s*<(?<type>.+)>)|
        (?<inherit>inherit)\s+((?<readonly>readonly)\s+)?(?<attribute>attribute)\s+(?<type>.+)\s+((?<required>required)|(?<item>.+))|
        (?<attribute>attribute)\s+(?<type>.+)\s+((?<required>required)|(?<item>.+))\s+(?<setraises>setraises)(\s*\(.*\))?|
        (?<attribute>attribute)\s+(?<type>.+)\s+((?<required>required)|(?<item>[^\(\s]+))|
        (?<maplike>maplike)\s*<(?<type>.+)>|
        (?<setlike>setlike)\s*<(?<type>.+)>|
        (?<type>.+?)\s+(?<item>[^\(\s]+)\s*(?<function>\((?<args>.*)\)))$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex MemberExtendedParser = new Regex(@"(exposed(\s*=\s*(?<exposed>(\([^\)]+\))|[^\(\s,\]]+)))(,|$)|
        (?<allowshared>allowshared)(,|$)|
        (?<cereactions>cereactions)(,|$)|
        (?<clamp>clamp)(,|$)|
        (?<default>default)(,|$)|
        (?<enforcerange>enforcerange)(,|$)|
        (?<lenientsetter>lenientsetter)(,|$)|
        (?<lenientthis>lenientthis)(,|$)|
        (?<newobject>newobject)(,|$)|
        (putforwards(\s*=\s*(?<putforwards>[^\s,\]]+)))(,|$)|
        (?<replaceable>replaceable)(,|$)|
        (?<sameobject>sameobject)(,|$)|
        (?<securecontext>securecontext)(,|$)|
        (treatnullas(\s*=\s*(?<treatnullas>[^\s,\]]+)))(,|$)|
        (treatundefinedas(\s*=\s*(?<treatundefinedas>[^\s,\]]+)))(,|$)|
        (?<unforgeable>unforgeable)(,|$)|
        (?<unscopable>unscopable)(,|$)|
        (?<pure>pure)(,|$)|
        (?<constant>constant)(,|$)|
        (internalbinding(\s*=\s*(?<internalbinding>[^\s,\]]+)))(,|$)|

        (storeinslot(\s*=\s*(?<storeinslot>[^\s,\]]+)))(,|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        public static IEnumerable<InterfaceType> GetAllInterfaces(string cleanString, SpecData specificationData)
        {
            var interfaces = new List<InterfaceType>();

            foreach (Match iface in InterfaceParser.Matches(cleanString))
            {
                var interfaceDefinition = new InterfaceType
                {
                    Name = iface.Groups["name"].Value.Trim(),
                    IsPartial = !string.IsNullOrWhiteSpace(iface.Groups["partial"].Value.Trim()),
                    IsCallback = !string.IsNullOrWhiteSpace(iface.Groups["callback"].Value.Trim()),
                    IsMixin = !string.IsNullOrWhiteSpace(iface.Groups["mixin"].Value.Trim()),
                    ExtendedAttribute = CleanString(iface.Groups["extended"].Value),
                    SpecNames = new[] { specificationData.Name }
                };

                if (!string.IsNullOrWhiteSpace(interfaceDefinition.ExtendedAttribute))
                {
                    var constructors = interfaceDefinition.Constructors.ToList();
                    var namedConstructors = interfaceDefinition.NamedConstructors.ToList();
                    var exposed = interfaceDefinition.Exposed.ToList();
                    var globals = interfaceDefinition.Globals.ToList();
                    var primaryGlobals = interfaceDefinition.PrimaryGlobals.ToList();
                    var legacyWindowAliases = interfaceDefinition.LegacyWindowAliases.ToList();
                    foreach (Match m in InterfaceExtendedParser.Matches(interfaceDefinition.ExtendedAttribute))
                    {
                        interfaceDefinition.IsGlobal = interfaceDefinition.IsGlobal || !string.IsNullOrWhiteSpace(m.Groups["global"].Value.Trim());
                        interfaceDefinition.HtmlConstructor = interfaceDefinition.HtmlConstructor || !string.IsNullOrWhiteSpace(m.Groups["htmlconstructor"].Value.Trim());
                        interfaceDefinition.ImplicitThis = interfaceDefinition.ImplicitThis || !string.IsNullOrWhiteSpace(m.Groups["implicitthis"].Value.Trim());
                        interfaceDefinition.ArrayClass = interfaceDefinition.ArrayClass || !string.IsNullOrWhiteSpace(m.Groups["arrayclass"].Value.Trim());
                        interfaceDefinition.LegacyArrayClass = interfaceDefinition.LegacyArrayClass || !string.IsNullOrWhiteSpace(m.Groups["legacyarrayclass"].Value.Trim());
                        interfaceDefinition.LegacyUnenumerableNamedProperties = interfaceDefinition.LegacyUnenumerableNamedProperties || !string.IsNullOrWhiteSpace(m.Groups["legacyunenumerablenamedproperties"].Value.Trim());

                        interfaceDefinition.IsLegacyWindowAlias = interfaceDefinition.IsLegacyWindowAlias || !string.IsNullOrWhiteSpace(m.Groups["legacywindowalias"].Value.Trim());

                        interfaceDefinition.NoInterfaceObject = interfaceDefinition.NoInterfaceObject || !string.IsNullOrWhiteSpace(m.Groups["nointerfaceobject"].Value.Trim());
                        interfaceDefinition.OverrideBuiltins = interfaceDefinition.OverrideBuiltins || !string.IsNullOrWhiteSpace(m.Groups["overridebuiltins"].Value.Trim());
                        interfaceDefinition.IsPrimaryGlobal = interfaceDefinition.IsPrimaryGlobal || !string.IsNullOrWhiteSpace(m.Groups["primaryglobal"].Value.Trim());
                        interfaceDefinition.SecureContext = interfaceDefinition.SecureContext || !string.IsNullOrWhiteSpace(m.Groups["securecontext"].Value.Trim());
                        interfaceDefinition.Unforgeable = interfaceDefinition.Unforgeable || !string.IsNullOrWhiteSpace(m.Groups["unforgeable"].Value.Trim());

                        var constructor = m.Groups["constructor"].Value.Trim();
                        if (!string.IsNullOrWhiteSpace(constructor))
                        {
                            constructors.Add(RegexLibrary.ParenCleaner.Replace(constructor, string.Empty));
                        }
                        var named = m.Groups["namedconstructor"].Value.Trim();
                        if (!string.IsNullOrWhiteSpace(named))
                        {
                            namedConstructors.Add(named);
                        }
                        var exposedValue = RegexLibrary.GroupingCleaner.Replace(m.Groups["exposed"].Value, string.Empty);
                        if (!string.IsNullOrWhiteSpace(exposedValue))
                        {
                            exposed.AddRange(exposedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim()));
                        }
                        var globalsValue = RegexLibrary.GroupingCleaner.Replace(m.Groups["globals"].Value, string.Empty);
                        if (!string.IsNullOrWhiteSpace(globalsValue))
                        {
                            globals.AddRange(globalsValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim()));
                        }
                        var primaryGlobalsValue = RegexLibrary.GroupingCleaner.Replace(m.Groups["primaryglobals"].Value, string.Empty);
                        if (!string.IsNullOrWhiteSpace(primaryGlobalsValue))
                        {
                            primaryGlobals.AddRange(primaryGlobalsValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim()));
                        }
                        var legacyWindowAliasValue = RegexLibrary.GroupingCleaner.Replace(m.Groups["legacywindowaliases"].Value, string.Empty);
                        if (!string.IsNullOrWhiteSpace(legacyWindowAliasValue))
                        {
                            legacyWindowAliases.AddRange(legacyWindowAliasValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim()));
                        }
                    }
                    interfaceDefinition.Constructors = constructors.Distinct();
                    interfaceDefinition.NamedConstructors = namedConstructors.Distinct();
                    interfaceDefinition.Exposed = exposed.Distinct();
                    interfaceDefinition.Globals = globals.Distinct();
                    interfaceDefinition.PrimaryGlobals = primaryGlobals.Distinct();
                    interfaceDefinition.LegacyWindowAliases = legacyWindowAliases.Distinct();
                }

                var inherits = iface.Groups["inherits"].Value;
                interfaceDefinition.Inherits = inherits.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim());
                interfaceDefinition.Members = iface.Groups["members"].Length > 0 ? GetAllInterfaceMembers(iface.Groups["members"].Value, specificationData, ref interfaceDefinition) : new List<Member>();

                if (!interfaces.Contains(interfaceDefinition))
                {
                    WebIdlBuilder.GenerateInterfaceFile(interfaceDefinition);
                    interfaces.Add(interfaceDefinition);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Duplicate interface: {interfaceDefinition.Name}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return interfaces;
        }

        private static IEnumerable<Member> GetAllInterfaceMembers(string memberItems, SpecData specificationData, ref InterfaceType interfaceDefinition)
        {
            var memberList = new List<Member>();

            memberItems = CleanString(memberItems);

            var members = memberItems.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim()).ToArray();

            foreach (var item in members.Where(item => !string.IsNullOrWhiteSpace(item)))
            {
                if (IndividualInterfaceMember.IsMatch(item))
                {
                    var m = IndividualInterfaceMember.Match(item);

                    var isProperty = !string.IsNullOrWhiteSpace(m.Groups["attribute"].Value) &&
                                    string.IsNullOrWhiteSpace(m.Groups["function"].Value) &&
                                    string.IsNullOrWhiteSpace(m.Groups["maplike"].Value) &&
                                    string.IsNullOrWhiteSpace(m.Groups["setlike"].Value) &&
                                    !(RegexLibrary.TypeCleaner.Replace(m.Groups["type"].Value, "?")
                                        .Trim())
                                        .Equals("function", StringComparison.OrdinalIgnoreCase);

                    var name = m.Groups["item"].Value.Trim();
                    //Skip special CSSOM definitions
                    if (new[] { "_camel_cased_attribute", "_webkit_cased_attribute", "_dashed_attribute" }.Contains(name))
                    {
                        continue;
                    }
                    if (Regex.IsMatch(name, "^_[a-z]", RegexOptions.IgnoreCase))
                    {
                        name = name.TrimStart('_');
                    }
                    var memberItem = new Member(name)
                    {
                        Type = RegexLibrary.OldTypeCleaner.Replace(RegexLibrary.TypeCleaner.Replace(m.Groups["type"].Value.Replace("≺", "<").Replace("≻", ">"), "?"), string.Empty).Trim(),
                        Args = m.Groups["args"].Value.Trim(),
                        Function = !string.IsNullOrWhiteSpace(m.Groups["function"].Value),
                        Attribute = !string.IsNullOrWhiteSpace(m.Groups["attribute"].Value),
                        ExtendedAttribute = m.Groups["extended"].Value.Trim(),
                        Static = !string.IsNullOrWhiteSpace(m.Groups["static"].Value),
                        HasGet = isProperty,
                        Getter = !string.IsNullOrWhiteSpace(m.Groups["getter"].Value),
                        Setter = !string.IsNullOrWhiteSpace(m.Groups["setter"].Value),
                        Creator = !string.IsNullOrWhiteSpace(m.Groups["creator"].Value),
                        Deleter = !string.IsNullOrWhiteSpace(m.Groups["deleter"].Value),
                        LegacyCaller = !string.IsNullOrWhiteSpace(m.Groups["legacycaller"].Value),
                        Stringifier = !string.IsNullOrWhiteSpace(m.Groups["stringifier"].Value),
                        Serializer = !string.IsNullOrWhiteSpace(m.Groups["serializer"].Value),
                        Inherit = !string.IsNullOrWhiteSpace(m.Groups["inherit"].Value),
                        MapLike = !string.IsNullOrWhiteSpace(m.Groups["maplike"].Value),
                        SetLike = !string.IsNullOrWhiteSpace(m.Groups["setlike"].Value),
                        Readonly = !string.IsNullOrWhiteSpace(m.Groups["readonly"].Value) || !string.IsNullOrWhiteSpace(m.Groups["setraises"].Value),
                        Required = !string.IsNullOrWhiteSpace(m.Groups["required"].Value),
                        Iterable = !string.IsNullOrWhiteSpace(m.Groups["iterable"].Value),
                        LegacyIterable = !string.IsNullOrWhiteSpace(m.Groups["legacyiterable"].Value),
                        Bracket = m.Groups["bracket"].Value.Trim(),
                        Const = !string.IsNullOrWhiteSpace(m.Groups["const"].Value),
                        Value = m.Groups["value"].Value.Trim(),
                        SpecNames = new[] { specificationData.Name }
                    };

                    if (!string.IsNullOrWhiteSpace(memberItem.ExtendedAttribute))
                    {
                        var exposed = new List<string>();
                        foreach (Match mep in MemberExtendedParser.Matches(memberItem.ExtendedAttribute))
                        {
                            memberItem.AllowShared = memberItem.AllowShared || !string.IsNullOrWhiteSpace(mep.Groups["allowshared"].Value.Trim());
                            memberItem.CeReactions = memberItem.CeReactions || !string.IsNullOrWhiteSpace(mep.Groups["cereactions"].Value.Trim());
                            memberItem.Clamp = memberItem.Clamp || !string.IsNullOrWhiteSpace(mep.Groups["clamp"].Value.Trim());
                            memberItem.Default = memberItem.Default || !string.IsNullOrWhiteSpace(mep.Groups["default"].Value.Trim());
                            memberItem.EnforceRange = memberItem.EnforceRange || !string.IsNullOrWhiteSpace(mep.Groups["enforcerange"].Value.Trim());
                            memberItem.LenientSetter = memberItem.LenientSetter || !string.IsNullOrWhiteSpace(mep.Groups["lenientsetter"].Value.Trim());
                            memberItem.LenientThis = memberItem.LenientThis || !string.IsNullOrWhiteSpace(mep.Groups["lenientthis"].Value.Trim());
                            memberItem.NewObject = memberItem.NewObject || !string.IsNullOrWhiteSpace(mep.Groups["newobject"].Value.Trim());
                            memberItem.PutForwards = mep.Groups["putforwards"].Value.Trim();
                            memberItem.Replaceable = memberItem.Replaceable || !string.IsNullOrWhiteSpace(mep.Groups["replaceable"].Value.Trim());
                            memberItem.SameObject = memberItem.SameObject || !string.IsNullOrWhiteSpace(mep.Groups["sameobject"].Value.Trim());
                            memberItem.SecureContext = memberItem.SecureContext || !string.IsNullOrWhiteSpace(mep.Groups["securecontext"].Value.Trim());
                            memberItem.TreatNullAs = mep.Groups["treatnullas"].Value.Trim();
                            memberItem.TreatUndefinedAs = mep.Groups["treatundefinedas"].Value.Trim();
                            memberItem.Unforgeable = memberItem.Unforgeable || !string.IsNullOrWhiteSpace(mep.Groups["unforgeable"].Value.Trim());
                            memberItem.Unscopable = memberItem.Unscopable || !string.IsNullOrWhiteSpace(mep.Groups["unscopable"].Value.Trim());

                            memberItem.Pure = memberItem.Unscopable || !string.IsNullOrWhiteSpace(mep.Groups["pure"].Value.Trim());
                            memberItem.Constant = memberItem.Unscopable || !string.IsNullOrWhiteSpace(mep.Groups["constant"].Value.Trim());
                            memberItem.StoreInSlot = mep.Groups["storeinslot"].Value.Trim();

                            var exposedValue = RegexLibrary.GroupingCleaner.Replace(mep.Groups["exposed"].Value, string.Empty);
                            if (!string.IsNullOrWhiteSpace(exposedValue))
                            {
                                exposed.AddRange(exposedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim()));
                            }
                        }
                        memberItem.Exposed = exposed.Distinct();
                    }

                    try
                    {
                        memberItem.ArgTypes = GetArgTypes(memberItem.Args);
                    }
                    catch (ArgumentException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Fail argument for member- {item}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;
                    }

                    memberItem.HasSet = (!memberItem.Readonly & isProperty) ||
                                        (memberItem.Readonly &&
                                         memberItem.ExtendedAttribute.IndexOf("replaceable", StringComparison.OrdinalIgnoreCase) > -1);

                    if (!memberList.Contains(memberItem))
                    {
                        memberList.Add(memberItem);
                    }
                }
                else
                {
                    //detect if its an interface extended attribute
                    if ((item.StartsWith("constructor", StringComparison.InvariantCultureIgnoreCase)) && (InterfaceExtendedParser.IsMatch(item)))
                    {
                        var constructors = interfaceDefinition.Constructors.ToList();
                        constructors.AddRange(from Match m in InterfaceExtendedParser.Matches(item) select m.Groups["constructor"].Value.Trim() into constructor where !string.IsNullOrWhiteSpace(constructor) select RegexLibrary.ParenCleaner.Replace(constructor, string.Empty));
                        interfaceDefinition.Constructors = constructors.Distinct();
                        continue;
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Fail member- {item}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return memberList.OrderBy(a => a.Name).ToList();
        }
    }
}