using System.IO;
using Newtonsoft.Json;

namespace MS.Internal.Generator.Core
{
    public class BaseSerializarionJson : ISerializationJson
    {
        public TU DeserializeJsonDataFile<TU>(string fileName)
        {
            if (!File.Exists(fileName)) { return default(TU); }
            using (var r = new StreamReader(fileName))
            {
                return JsonConvert.DeserializeObject<TU>(r.ReadToEnd());
            }
        }

        public string SerializeObject<T>(T jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject);
        }
    }
}