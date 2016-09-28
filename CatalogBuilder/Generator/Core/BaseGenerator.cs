using System;
using System.IO;
using Newtonsoft.Json;

namespace MS.Internal.Generator.Core
{
    public class BaseGenerator : IGenerator
    {
        public string OutputPath { get; set; }

        public T DeserializeJsonDataFile<T>(string fileName)
        {
            if (!File.Exists(fileName)) { return default(T); }
            using (var r = new StreamReader(fileName))
            {
                return JsonConvert.DeserializeObject<T>(r.ReadToEnd());
            }
        }

        public virtual T GenerateSpecificDataObject<T, TU>(TU dataObject)
        {
            throw new NotImplementedException();
        }

        public virtual T ProcessJsonObject<T, TU>(TU jsonObject)
        {
            throw new NotImplementedException();
        }

        public string SerializeObject<T>(T jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject);
        }

        public void Write(string jsonString)
        {
            Write(OutputPath, jsonString);
        }

        public void Write(string path, string jsonString)
        {
            File.WriteAllText(path, jsonString);
        }
    }
}