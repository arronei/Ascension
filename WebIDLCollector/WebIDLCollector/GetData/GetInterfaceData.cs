using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.GetData
{
    public partial class DataCollectors
    {
        private static readonly Regex InterfaceParser = new Regex(@"(\[(?<extended>[^\]]+)\]\s*)?
        (((?<partial>partial)|(?<callback>callback))\s+)?(/\*[^\*]+\*/\s*)?interface\s+(/\*[^\*]+\*/\s*)?
        (?<name>\w+)(?:\s*:\s*(?<inherits>[\w,\s]+))?\s*\{\s*(?<members>(.*?\{.*?\};.*?|[^\}]+))?\s*\};?", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

        private static readonly Regex InterfaceExtendedParser = new Regex(@"((?<constructor>constructor(\s*\((?<args>.+?)?\))?)(,|$)|
        (?<namedconstructor>namedconstructor\s*=\s*[^\)\s,\]]+(\s*\((?<ncargs>.+)?\))?)(,|$)|
        (exposed(\s*=\s*(?<exposed>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<global>global(\s*=\s*(?<globals>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<implicitthis>implicitthis)(,|$)|
        [^y](?<arrayclass>arrayclass)(,|$)|
        (?<legacyarrayclass>legacyarrayclass)(,|$)|
        (?<legacyunenumerablenamedproperties>legacyunenumerablenamedproperties)(,|$)|
        (?<nointerfaceobject>nointerfaceobject)(,|$)|
        (?<overridebuiltins>overridebuiltins)(,|$)|
        (?<primaryglobal>primaryglobal(\s*=\s*(?<primaryglobals>(\([^\)]+\))|[^\(\s,\]]+))?)(,|$)|
        (?<unforgeable>unforgeable)(,|$))+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex IndividualMember = new Regex(@"^\s*(\[(?<extended>[^\]]+)]\s*)?(
        ((((?<getter>getter)|(?<setter>setter)|(?<creator>creator)|(?<deleter>deleter)|(?<legacycaller>legacycaller))\s+){1,5}\s*)(?<type>.+)\s+(?<item>[^(\s]+)?\s*(?<function>\((?<args>.*)\))|
        (?<const>const)\s+(?<type>.+)\s+(?<item>.+?)\s*=\s*(?<value>.+?)|
        (?<serializer>serializer)\s*((?<type>.+)\s+((?<item>[^\(\s]+)\s*)?(?<function>\((?<args>.*)\))|=\s*(?<bracket>[\{\[])?\s*(?<value>[^\}\]]*)\s*[\}\]]?)?|
        (?<stringifier>stringifier)((\s+((?<readonly>readonly)\s+)?(?<attribute>attribute)\s+(?<type>.+)(\s+(?<required>required))?(\s+(?<item>[^;]+)))|(\s+(?<type>.+))(\s+(?<item>.*))(?<function>\((?<args>.*)\)))?$|
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
        (?<clamp>clamp)(,|$)|
        (?<enforcerange>enforcerange)(,|$)|
        (?<lenientthis>lenientthis)(,|$)|
        (?<newobject>newobject)(,|$)|
        (putforwards(\s*=\s*(?<putforwards>[^\s,\]]+)))(,|$)|
        (?<replaceable>replaceable)(,|$)|
        (?<sameobject>sameobject)(,|$)|
        (treatnullas(\s*=\s*(?<treatnullas>[^\s,\]]+)))(,|$)|
        (?<unforgeable>unforgeable)(,|$)|
        (?<unscopeable>unscopeable)(,|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex ArgumentParser = new Regex(@"^\s*(\[(?<extended>[^\]]+)]\s*)?(
        ((?<in>in)\s+)?
        ((?<optional>optional)\s+)?
        (?<type>[^\.=]+)
        (\s*(?<ellipsis>\.\.\.))?\s+
        ((?<name>[^=\s]+)
        (\s*=\s*(?<value>.+?))?))$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex AttributeExtendedParser = new Regex(@"(?<clamp>clamp)(,|$)|
        (?<enforcerange>enforcerange)(,|$)|
        (treatnullas(\s*=\s*(?<treatnullas>[^\s,\]]+)))(,|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

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
                    foreach (Match m in InterfaceExtendedParser.Matches(interfaceDefinition.ExtendedAttribute))
                    {
                        interfaceDefinition.IsGlobal = !string.IsNullOrWhiteSpace(m.Groups["global"].Value.Trim());
                        interfaceDefinition.ImplicitThis = !string.IsNullOrWhiteSpace(m.Groups["implicitthis"].Value.Trim());
                        interfaceDefinition.ArrayClass = !string.IsNullOrWhiteSpace(m.Groups["arrayclass"].Value.Trim());
                        interfaceDefinition.LegacyArrayClass = !string.IsNullOrWhiteSpace(m.Groups["legacyarrayclass"].Value.Trim());
                        interfaceDefinition.LegacyUnenumerableNamedProperties = !string.IsNullOrWhiteSpace(m.Groups["legacyunenumerablenamedproperties"].Value.Trim());
                        interfaceDefinition.NoInterfaceObject = !string.IsNullOrWhiteSpace(m.Groups["nointerfaceobject"].Value.Trim());
                        interfaceDefinition.OverrideBuiltins = !string.IsNullOrWhiteSpace(m.Groups["overridebuiltins"].Value.Trim());
                        interfaceDefinition.IsPrimaryGlobal = !string.IsNullOrWhiteSpace(m.Groups["primaryglobal"].Value.Trim());
                        interfaceDefinition.Unforgeable = !string.IsNullOrWhiteSpace(m.Groups["unforgeable"].Value.Trim());

                        var constructor = m.Groups["constructor"].Value.Trim();
                        if (!string.IsNullOrWhiteSpace(constructor))
                        {
                            constructors.Add(Regex.Replace(constructor, @"\(\)", string.Empty));
                        }
                        var named = m.Groups["namedconstructor"].Value.Trim();
                        if (!string.IsNullOrWhiteSpace(named))
                        {
                            namedConstructors.Add(named);
                        }
                        var exposedValue = Regex.Replace(m.Groups["exposed"].Value, @"[\(\)\s]+", string.Empty);
                        if (!string.IsNullOrWhiteSpace(exposedValue))
                        {
                            exposed.AddRange(exposedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim()));
                        }
                        var globalsValue = Regex.Replace(m.Groups["globals"].Value, @"[\(\)\s]+", string.Empty);
                        if (!string.IsNullOrWhiteSpace(globalsValue))
                        {
                            globals.AddRange(globalsValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim()));
                        }
                        var primaryGlobalsValue = Regex.Replace(m.Groups["primaryglobals"].Value, @"[\(\)\s]+", string.Empty);
                        if (!string.IsNullOrWhiteSpace(primaryGlobalsValue))
                        {
                            primaryGlobals.AddRange(primaryGlobalsValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim()));
                        }
                    }
                    interfaceDefinition.Constructors = constructors.Distinct();
                    interfaceDefinition.NamedConstructors = namedConstructors.Distinct();
                    interfaceDefinition.Exposed = exposed.Distinct();
                    interfaceDefinition.Globals = globals.Distinct();
                    interfaceDefinition.PrimaryGlobals = primaryGlobals.Distinct();
                }

                var inherits = iface.Groups["inherits"].Value;
                interfaceDefinition.Inherits = inherits.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(api => api.Trim());
                interfaceDefinition.Members = iface.Groups["members"].Length > 0 ? GetAllInterfaceMembers(iface.Groups["members"].Value, specificationData, ref interfaceDefinition) : new List<Member>();

                if (!interfaces.Contains(interfaceDefinition))
                {
                    interfaces.Add(interfaceDefinition);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Duplicate interface: " + interfaceDefinition.Name);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return interfaces;
        }

        private static string CleanString(string value)
        {
            value = value.Trim().Trim('.');
            value = Regex.Replace(value, @"\s*//.*$", string.Empty, RegexOptions.Multiline);
            value = Regex.Replace(value, @"\s+", " ");
            value = Regex.Replace(value, @"\s*(set)?raises\([^)]*?\)\s*;", ";");
            value = Regex.Replace(value, @"(?<start>(\(|,)\s*)in\s+", "${start}");
            return value.Trim();
        }

        private static IEnumerable<Member> GetAllInterfaceMembers(string memberItems, SpecData specificationData, ref InterfaceType interfaceDefinition)
        {
            var memberList = new List<Member>();

            memberItems = CleanString(memberItems);

            var members = memberItems.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim()).ToArray();

            foreach (var item in members.Where(item => !string.IsNullOrWhiteSpace(item)))
            {
                if (IndividualMember.IsMatch(item))
                {
                    var m = IndividualMember.Match(item);

                    var isProperty = !string.IsNullOrWhiteSpace(m.Groups["attribute"].Value) &&
                                    string.IsNullOrWhiteSpace(m.Groups["function"].Value) &&
                                    string.IsNullOrWhiteSpace(m.Groups["maplike"].Value) &&
                                    string.IsNullOrWhiteSpace(m.Groups["setlike"].Value) &&
                                    !(Regex.Replace(m.Groups["type"].Value, @"\s+\?", "?")
                                        .Trim())
                                        .Equals("function", StringComparison.OrdinalIgnoreCase);

                    var name = m.Groups["item"].Value.Trim();
                    //Skip special CSSOM definitions
                    if (new[] { "_camel_cased_attribute", "_webkit_cased_attribute", "_dashed_attribute" }.Contains(name))
                    {
                        continue;
                    }
                    var memberItem = new Member(name)
                    {
                        Type = Regex.Replace(Regex.Replace(m.Groups["type"].Value.Replace("≺", "<").Replace("≻", ">"), @"\s+\?", "?"), @"[a-z]*::", string.Empty).Trim(),
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
                            memberItem.Clamp = !string.IsNullOrWhiteSpace(mep.Groups["clamp"].Value.Trim());
                            memberItem.EnforceRange = !string.IsNullOrWhiteSpace(mep.Groups["enforcerange"].Value.Trim());
                            memberItem.LenientThis = !string.IsNullOrWhiteSpace(mep.Groups["lenientthis"].Value.Trim());
                            memberItem.NewObject = !string.IsNullOrWhiteSpace(mep.Groups["newobject"].Value.Trim());
                            memberItem.PutForwards = mep.Groups["putforwards"].Value.Trim();
                            memberItem.Replaceable = !string.IsNullOrWhiteSpace(mep.Groups["replaceable"].Value.Trim());
                            memberItem.SameObject = !string.IsNullOrWhiteSpace(mep.Groups["sameobject"].Value.Trim());
                            memberItem.TreatNullAs = mep.Groups["treatnullas"].Value.Trim();
                            memberItem.Unforgeable = !string.IsNullOrWhiteSpace(mep.Groups["unforgeable"].Value.Trim());
                            memberItem.Unscopeable = !string.IsNullOrWhiteSpace(mep.Groups["unscopeable"].Value.Trim());

                            var exposedValue = Regex.Replace(mep.Groups["exposed"].Value, @"[\(\)\s]+", string.Empty);
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
                        Console.WriteLine("Fail argument for member- " + item);
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
                        foreach (Match m in InterfaceExtendedParser.Matches(item))
                        {
                            var constructor = m.Groups["constructor"].Value.Trim();
                            if (!string.IsNullOrWhiteSpace(constructor))
                            {
                                constructors.Add(Regex.Replace(constructor, @"\(\)", string.Empty));
                            }
                        }
                        interfaceDefinition.Constructors = constructors.Distinct();
                        continue;
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fail member- " + item);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return memberList.OrderBy(a => a.Name).ToList();
        }

        private static IEnumerable<Argument> GetArgTypes(string args)
        {
            var arguments = args.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var argumentList = new List<Argument>();

            foreach (var argument in arguments.Select(argument => argument.Trim()))
            {
                if (ArgumentParser.IsMatch(argument))
                {
                    var m = ArgumentParser.Match(argument);

                    var argumentItem = new Argument(m.Groups["name"].Value.Trim())
                    {
                        Type = Regex.Replace(Regex.Replace(m.Groups["type"].Value, @"\s+\?", "?"), @"[a-z]*::", "").Trim(),
                        ExtendedAttribute = m.Groups["extended"].Value.Trim(),
                        In = !string.IsNullOrWhiteSpace(m.Groups["in"].Value),
                        Optional = !string.IsNullOrWhiteSpace(m.Groups["optional"].Value),
                        Ellipsis = !string.IsNullOrWhiteSpace(m.Groups["ellipsis"].Value),
                        Value = m.Groups["value"].Value.Trim()
                    };

                    if (!string.IsNullOrWhiteSpace(argumentItem.ExtendedAttribute))
                    {
                        foreach (Match aep in AttributeExtendedParser.Matches(argumentItem.ExtendedAttribute))
                        {
                            argumentItem.Clamp = !string.IsNullOrWhiteSpace(aep.Groups["clamp"].Value.Trim());
                            argumentItem.EnforceRange = !string.IsNullOrWhiteSpace(aep.Groups["enforcerange"].Value.Trim());
                            argumentItem.TreatNullAs = aep.Groups["treatnullas"].Value.Trim();
                        }
                    }

                    argumentList.Add(argumentItem);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid argument- " + argument);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    throw new ArgumentException();
                }
            }

            return argumentList;
        }
    }
}