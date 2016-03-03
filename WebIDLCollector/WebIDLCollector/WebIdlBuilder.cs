using System;
using System.IO;
using System.Linq;
using System.Text;
using WebIDLCollector.IDLTypes;

namespace WebIDLCollector
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
            Console.WriteLine("Generating WebIdl - " + _specData.Name);
            var webidlString = CreateWebIdl(_specData);
            using (var file = new StreamWriter("webidl/" + _specData.Name + ".webidl"))
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
                finalRecreate.Append(enumerationType.Reconstruct).AppendLine();
            }

            foreach (var typeDefType in specData.TypeDefs)
            {
                finalRecreate.Append(typeDefType.Reconstruct).AppendLine();
            }

            foreach (var dictionaryType in specData.Dictionaries)
            {
                finalRecreate.Append(dictionaryType.Reconstruct(_showSpecNames)).AppendLine();
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
                finalRecreate.Append(callbackType.Reconstruct).AppendLine();
            }

            return finalRecreate.ToString().Trim();
        }
    }
}