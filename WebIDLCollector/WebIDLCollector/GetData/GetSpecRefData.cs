using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using WebIDLCollector.SpecRef;

namespace WebIDLCollector.GetData
{
    public partial class DataCollectors
    {
        public static SortedDictionary<string, SpecRefType> GetSpecRefData()
        {
            Console.WriteLine("Retrieve SpecRef data...");
            var request = WebRequest.Create("https://specref.herokuapp.com/bibrefs");
            var speRefData = new SortedDictionary<string, SpecRefType>();
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unable to get SpecRef data.");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.ReadKey();
                        return null;
                    }
                    using (var stream = new StreamReader(responseStream))
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

                            speRefData.Add(shortName, specRef);
                        }
                    }
                }
            }

            return speRefData;
        }
    }
}
