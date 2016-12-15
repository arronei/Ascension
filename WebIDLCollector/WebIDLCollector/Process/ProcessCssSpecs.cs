using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using WebIDLCollector.IDLTypes;
using WebIDLCollector.Utilities;

namespace WebIDLCollector.Process
{
    public static partial class ProcessSpecs
    {
        public static void ProcessSvGCss(IParentNode items, SpecData specificationData)
        {
            var properties = BuildSvgPropertyList(items);

            specificationData.Interfaces.AddRange(GenerateInterfacesForCss(properties, specificationData));
        }

        private static IEnumerable<Property> BuildSvgPropertyList(IParentNode table)
        {
            var properties = new List<Property>();
            var t = table.QuerySelector("thead").QuerySelectorAll("th").Select(b => b.TextContent.Trim()).ToList();
            foreach (var element in table.QuerySelector("tbody").QuerySelectorAll("tr"))
            {
                var p = new Property();
                for (var i = 0; i < t.Count; i++)
                {
                    var f = element.QuerySelectorAll("th, td")[i].TextContent.Trim();
                    p.GetType().GetProperty(FixPropertyName(t[i])).SetValue(p, f, null);
                }

                AddSingleOrMultipleProperties(p, properties);
            }

            return properties;
        }

        public static void ProcessCss(IEnumerable<IElement> items, SpecData specificationData)
        {
            var properties = BuildCssPropertyList(items);

            specificationData.Interfaces.AddRange(GenerateInterfacesForCss(properties, specificationData));
        }

        private static IEnumerable<Property> BuildCssPropertyList(IEnumerable<IElement> items)
        {
            var properties = new List<Property>();
            foreach (var propdef in items)
            {
                var p = new Property
                {
                    Name = propdef.QuerySelector("dt").TextContent.Trim()
                };

                foreach (var row in propdef.QuerySelectorAll("dd table.propinfo tr"))
                {
                    var th = row.QuerySelector("td").TextContent.Trim().TrimEnd(':');
                    var td = RegexLibrary.WhitespaceCleaner.Replace(row.QuerySelectorAll("td")[1].TextContent.Trim(), " ");

                    p.GetType().GetProperty(FixPropertyName(th)).SetValue(p, td, null);
                }

                AddSingleOrMultipleProperties(p, properties);
            }

            return properties;
        }

        public static void ProcessBikeshed(IEnumerable<IElement> items, SpecData specificationData)
        {
            var properties = BuildBikeshedPropertyList(items);

            specificationData.Interfaces.AddRange(GenerateInterfacesForCss(properties, specificationData));
        }

        private static IEnumerable<Property> BuildBikeshedPropertyList(IEnumerable<IElement> items)
        {
            var properties = new List<Property>();
            foreach (var table in items)
            {
                var p = new Property();

                foreach (var row in table.QuerySelectorAll("tr"))
                {
                    IElement tdAsThElement = null;
                    var thElement = row.QuerySelector("th") ?? (tdAsThElement = row.QuerySelector("td"));
                    var th = thElement.TextContent.Trim().TrimEnd(':', '>');

                    string text = null;
                    var comma = string.Empty;
                    foreach (var element in row.QuerySelectorAll("td dfn"))
                    {
                        text += comma + element.FirstChild?.TextContent;
                        comma = ", ";
                    }
                    if (tdAsThElement == null)
                    {
                        text = text ?? row.QuerySelector("td").TextContent;
                    }
                    else
                    {
                        text = text ?? row.QuerySelectorAll("td")[1].TextContent;
                    }
                    var td = RegexLibrary.WhitespaceCleaner.Replace(text.Trim(), " ");

                    p.GetType().GetProperty(FixPropertyName(th)).SetValue(p, td, null);
                }

                AddSingleOrMultipleProperties(p, properties);
            }

            return properties;
        }

        private static void AddSingleOrMultipleProperties(Property p, ICollection<Property> properties)
        {
            //determine if multiple properties
            var d = p.Name.Split(',', '/', '\n');
            if (d.Length > 1)
            {
                foreach (var item in d)
                {
                    var newProp = p.DeepClone();
                    newProp.Name = RegexLibrary.PropertyCleaner.Replace(item, string.Empty);
                    //add prop
                    properties.Add(newProp);
                }
            }
            else
            {
                //Fix Name
                p.Name = RegexLibrary.PropertyCleaner.Replace(p.Name, string.Empty);
                //Just add it
                properties.Add(p);
            }
        }

        private static IEnumerable<InterfaceType> GenerateInterfacesForCss(IEnumerable<Property> properties, SpecData specificationData)
        {
            var interfaces = new List<InterfaceType>();

            var interfaceDefinition = new InterfaceType
            {
                Name = "CSSStyleDeclaration",
                IsPartial = true,
                SpecNames = new[] { specificationData.Name }
            };

            var memberList = new List<Member>();
            foreach (var property in properties)
            {
                var memberOmItem = CreateCssOmMember(property.OmName, specificationData);
                if (!memberList.Contains(memberOmItem))
                {
                    memberList.Add(memberOmItem);
                }
                var memberPropertyItem = CreateCssOmMember(property.Name, specificationData);
                if (!memberList.Contains(memberPropertyItem))
                {
                    memberList.Add(memberPropertyItem);
                }
                if (string.IsNullOrWhiteSpace(property.WebkitOmName))
                {
                    continue;
                }
                var memberWebkitItem = CreateCssOmMember(property.WebkitOmName, specificationData);
                if (!memberList.Contains(memberWebkitItem))
                {
                    memberList.Add(memberWebkitItem);
                }
            }

            interfaceDefinition.Members = memberList;
            interfaces.Add(interfaceDefinition);
            return interfaces;
        }

        private static Member CreateCssOmMember(string name, SpecData specificationData)
        {
            return new Member(name)
            {
                ExtendedAttribute = "TreatNullAs=EmptyString",
                TreatNullAs = "EmptyString",
                Type = "DOMString",
                Attribute = true,
                HasGet = true,
                HasSet = true,
                SpecNames = new[] { specificationData.Name }
            };
        }

        private static string FixPropertyName(string value)
        {
            value = Regex.Replace(value, @"\s+\b([a-z])", match => match.Groups[1].Value.ToUpperInvariant()).Replace(" ", string.Empty);

            switch (value)
            {
                case "Inh.":
                    value = "Inherited";
                    break;
                case "%ages":
                    value = "Percentages";
                    break;
                case "Properties":
                    value = "Name";
                    break;
                case "Property":
                    value = "Name";
                    break;
                case "NewValues":
                    value = "NewValue";
                    break;
                case "InitialValue":
                    value = "Initial";
                    break;
                case "Values":
                    value = "Value";
                    break;
                case "Anim.":
                    value = "Animatable";
                    break;
                case "AminationType":
                    value = "AnimationType";
                    break;
                default:
                    throw new ArgumentException();
            }

            return value;
        }
    }
}