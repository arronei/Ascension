using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MS.Internal
{
    public class DataCollector
    {
        public static Dictionary<string, SpecRefType> GetSpecRefData()
        {
            Console.WriteLine("Retrieve SpecRef data...");
            var specRefData = new Dictionary<string, SpecRefType>();

            using (var sr = File.OpenRead("./data/bibrefs.json"))
            {
                DeserializeSpecRefData(sr, specRefData);
            }

            //var request = WebRequest.Create("https://specref.herokuapp.com/bibrefs");
            //using (var response = request.GetResponse())
            //{
            //    using (var jsonStream = response.GetResponseStream())
            //    {
            //        if (jsonStream == null)
            //        {
            //            Console.ForegroundColor = ConsoleColor.Red;
            //            Console.WriteLine("Unable to get SpecRef data.");
            //            Console.ForegroundColor = ConsoleColor.Gray;
            //            Console.ReadKey();
            //            return null;
            //        }
            //        DeserializeSpecRefData(jsonStream, specRefData);
            //    }
            //}

            return specRefData.Where(a => a.Value.AliasOf == null && a.Value.Href != null).ToDictionary(a => a.Key, b => b.Value);
        }

        private static void DeserializeSpecRefData(Stream jsonStream, IDictionary<string, SpecRefType> specRefData)
        {
            using (var stream = new StreamReader(jsonStream))
            {
                var specRefSerializer = new JsonSerializer();
                var specRefDictionary = (IDictionary<string, dynamic>)specRefSerializer.Deserialize(stream, typeof(IDictionary<string, dynamic>));
                foreach (var item in specRefDictionary)
                {
                    var shortName = item.Key;

                    SpecRefType specRef;
                    if (item.Value is string)
                    {
                        specRef = new SpecRefType
                        {
                            Data = item.Value
                        };
                    }
                    else
                    {
                        specRef = new SpecRefType
                        {
                            AliasOf = item.Value["aliasOf"],
                            Date = item.Value["date"],
                            Href = item.Value["href"],
                            Title = item.Value["title"]
                        };
                    }

                    specRefData.Add(shortName, specRef);
                }
            }
        }
    }
}
