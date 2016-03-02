using System;
using System.IO;

namespace WebIDLCollector
{
    public class JsonDataBuilder
    {
        private readonly bool _showSpecNames;
        private readonly SpecData _specData;

        public JsonDataBuilder(SpecData allCombinedSpecData, bool showSpecNames = false)
        {
            _specData = allCombinedSpecData;
            _showSpecNames = showSpecNames;
        }

        public void GenerateFile()
        {
            Console.WriteLine("Creating JsonObject");
            var jsonString = CreateJsonObject(_specData);
            using (var file = new StreamWriter("SpecMirror.js"))
            {
                file.WriteLine("{");
                file.WriteLine("\"browserVersion\": \"Specifications\", ");
                file.WriteLine("\"timestamp\": " + "\"" + DateTime.Now + "\",");
                file.WriteLine("\"types\": {");
                file.WriteLine(jsonString);
                file.WriteLine("}");
                file.WriteLine("}");
            }

            Console.WriteLine("Done writing.");
        }

        private object CreateJsonObject(SpecData _specData)
        {
            throw new NotImplementedException();
        }
    }
}