using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace TypeSystem.Data.Core
{
    public class BaseSerializarionJson<T>
    {
        public T DeserializeJsonData(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File ({fileName}) does not exist.");
                Console.ForegroundColor = ConsoleColor.Gray;
                return default;
            }
            using (var streamReader = new StreamReader(fileName))
            {
                return JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
            }
        }

        public T DeserializeJsonData(Uri url)
        {
            var request = WebRequest.Create(url);
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"URL ({url}) data not available.");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        return default;
                    }
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        return JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
                    }

                }
            }
        }

        protected string SerializeObject(object jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject);
        }
    }
}