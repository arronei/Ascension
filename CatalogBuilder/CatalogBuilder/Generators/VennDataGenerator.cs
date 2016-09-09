using System;
using System.IO;
using Newtonsoft.Json;

namespace MS.Internal.Generators
{
    public class VennDataGenerator : BaseGenerator, IGenerator
    {
        public string OutputPath { get; set; }
        public T GenerateJsonObject<T, TU>(TU dataObject)
        {
            throw new NotImplementedException();
        }

        public string SerializeObject<T>(T jsonObject)
        {
            throw new NotImplementedException();
        }

        public void WriteFile(string jsonString)
        {
            throw new NotImplementedException();
        }

        public void WriteFile(string path, string jsonString)
        {
            throw new NotImplementedException();
        }
    }

    public class BaseGenerator : IDeserializeJson
    {
        public TU DeserializeJsonDataFile<TU>(string fileName)
        {
            if (!File.Exists(fileName)) { return default(TU); }
            using (var r = new StreamReader(fileName))
            {
                return JsonConvert.DeserializeObject<TU>(r.ReadToEnd());
            }
        }
    }
}
