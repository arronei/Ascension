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

        public void GenerateFile()
        {
            const string webidlDirectory = "webidl";
            var webidlFile = webidlDirectory + "/" + _specData.Name + ".webidl";
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

        private string CreateWebIdl(SpecData specData)
        {
            var finalRecreate = new StringBuilder();

            foreach (var enumerationType in specData.Enumerations)
            {
                finalRecreate.AppendLine(enumerationType.Reconstruct);
            }

            foreach (var typeDefType in specData.TypeDefs)
            {
                finalRecreate.AppendLine(typeDefType.Reconstruct).AppendLine();
            }

            foreach (var dictionaryType in specData.Dictionaries)
            {
                finalRecreate.AppendLine(dictionaryType.Reconstruct(_showSpecNames));
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
                        finalRecreate.AppendLine(implements.Reconstruct);
                    }
                }
                finalRecreate.AppendLine();
            }

            foreach (var callbackType in specData.Callbacks)
            {
                finalRecreate.AppendLine(callbackType.Reconstruct).AppendLine();
            }

            return finalRecreate.ToString().Trim();
        }
    }
}