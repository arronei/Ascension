using System;
using TypeSystem.Data.Core;

namespace TypeSystem.Data.Browser
{
    public class Browser : BaseSerializarionJson<BrowsersCollection>
    {
        public BrowsersCollection DeserializeJsonDataFile(string browserReleaseFilePath)
        {
            Console.WriteLine("Retrieve Browser data...");

            return DeserializeJsonData(browserReleaseFilePath);
        }
    }
}