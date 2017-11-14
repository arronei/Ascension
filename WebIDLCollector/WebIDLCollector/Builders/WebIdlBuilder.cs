using System;
using System.IO;
using System.Linq;
using System.Text;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector.Builders
{
    public class WebIdlBuilder
    {
        private readonly bool _showSpecNames;
        private readonly SpecData _specData;

        public WebIdlBuilder(SpecData specData, bool showSpecNames = false)
        {
            _specData = specData;
            _showSpecNames = showSpecNames;
        }

        public void GenerateFile(string fileName = null)
        {
            const string webidlDirectory = "webidl";
            fileName = fileName ?? _specData.Name;
            var webidlFile = webidlDirectory + "/" + fileName + ".webidl";
            if (!Directory.Exists(webidlDirectory))
            {
                Directory.CreateDirectory(webidlDirectory);
            }

            Console.WriteLine("Generating WebIdl - " + _specData.Name);
            var webidlString = CreateWebIdl(_specData);
            using (var file = new StreamWriter(webidlFile, true))
            {
                file.WriteLine("// Last Generated: " + "\"" + DateTime.Now + "\",");
                file.Write(webidlString);
            }
        }

        public static void GenerateInterfaceFile(InterfaceType interfaceDefinition)
        {
            //const string webidlDirectory = "webidl/interfaces";
            //var webidlFile = webidlDirectory + "/" + interfaceDefinition.Name + ".webidl";
            //if (!Directory.Exists(webidlDirectory))
            //{
            //    Directory.CreateDirectory(webidlDirectory);
            //}

            //var name = interfaceDefinition.Name;
            //Console.WriteLine("Generating interface WebIdl - " + name);

            //var finalRecreate = new StringBuilder();
            //finalRecreate.AppendLine().Append(interfaceDefinition.Reconstruct());

            //if (File.Exists(webidlFile))
            //{
            //    File.AppendAllText(webidlFile, finalRecreate.ToString().TrimEnd());
            //}
            //else
            //{
            //    using (var file = new StreamWriter(webidlFile, true))
            //    {
            //        file.WriteLine("// Last Generated: " + "\"" + DateTime.Now + "\",");
            //        file.Write(finalRecreate.ToString().Trim());
            //    }
            //}
        }

        private string CreateWebIdl(SpecData specData)
        {
            var finalRecreate = new StringBuilder();

            foreach (var enumerationType in specData.Enumerations)
            {
                finalRecreate.AppendLine(enumerationType.Reconstruct(_showSpecNames));
            }

            foreach (var typeDefType in specData.TypeDefs)
            {
                finalRecreate.AppendLine(typeDefType.Reconstruct(_showSpecNames));
            }

            foreach (var callbackType in specData.Callbacks)
            {
                finalRecreate.AppendLine(callbackType.Reconstruct(_showSpecNames));
            }

            foreach (var dictionaryType in specData.Dictionaries)
            {
                finalRecreate.AppendLine(dictionaryType.Reconstruct(_showSpecNames));
            }

            foreach (var namespaceType in specData.Namespaces)
            {
                finalRecreate.AppendLine(namespaceType.Reconstruct(_showSpecNames));
            }

            foreach (var interfaceType in specData.Interfaces)
            {
                var name = interfaceType.Name;
                finalRecreate.Append(interfaceType.Reconstruct(_showSpecNames));
                if (specData.Implements.Exists(a => a.DestinationInterface.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    var thisTypeImplements =
                        specData.Implements.FindAll(
                            a => a.DestinationInterface.Equals(name, StringComparison.OrdinalIgnoreCase))
                            .Distinct(new ImplementsCompare())
                            .OrderBy(s => s.OriginatorInterface)
                            .ToList();
                    foreach (var implements in thisTypeImplements)
                    {
                        finalRecreate.AppendLine(implements.Reconstruct(_showSpecNames));
                    }
                }
                finalRecreate.AppendLine();
            }

            return finalRecreate.ToString().Trim();
        }
    }
}