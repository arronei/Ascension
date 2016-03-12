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
                    var td = Regex.Replace(row.QuerySelectorAll("td")[1].TextContent.Trim(), @"\s+", " ");

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
                    string th;
                    string td;
                    try
                    {
                        th = row.QuerySelector("th").TextContent.Trim().TrimEnd(':', '>');
                        var text = row.QuerySelector("td dfn")?.FirstChild?.TextContent ?? row.QuerySelector("td").TextContent;
                        td = Regex.Replace(text.Trim(), @"\s+", " ");
                    }
                    catch
                    {
                        th = row.QuerySelector("td").TextContent.Trim().TrimEnd(':', '>');
                        var text = row.QuerySelectorAll("td dfn")[0]?.FirstChild?.TextContent ?? row.QuerySelectorAll("td")[1].TextContent;
                        td = Regex.Replace(text.Trim(), @"\s+", " ");
                    }

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
                    newProp.Name = Regex.Replace(item, @"[^-a-z]", string.Empty);
                    //add prop
                    properties.Add(newProp);
                }
            }
            else
            {
                //Fix Name
                p.Name = Regex.Replace(p.Name, @"[^-a-z]", string.Empty);
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
            foreach (var memberItem in properties.Select(property => new Member(property.OmName)
            {
                Type = "DOMString",
                Attribute = true,
                HasGet = true,
                HasSet = true,
                SpecNames = new[] { specificationData.Name }
            }).Where(memberItem => !memberList.Contains(memberItem)))
            {
                memberList.Add(memberItem);
            }

            interfaceDefinition.Members = memberList.Count > 0 ? memberList : new List<Member>();
            interfaces.Add(interfaceDefinition);
            return interfaces;
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
            }

            return value;
        }
    }
}