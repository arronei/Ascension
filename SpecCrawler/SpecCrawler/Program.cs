using System.Collections.Generic;
using AngleSharp;

namespace MS.Internal
{
    public class SpecCrawler
    {
        public static void Main(string[] args)
        {
            var SpecList = DataCollector.GetSpecRefData();

            var SpecDataList = new List<SpecData>();

            foreach (var specRefItem in SpecList)
            {
                var shortName = specRefItem.Key;
                var date = specRefItem.Value.Date;
                var specUrl = specRefItem.Value.Href;

                var status = specRefItem.Value.Status;
                var obsoletedBy = specRefItem.Value.ObsoletedBy;
                var versions = specRefItem.Value.Versions;
            }
        }

        public static async void GetData(string specUrl)
        {
            var config = Configuration.Default.WithJavaScript();
            var document = await BrowsingContext.New(config).OpenAsync(specUrl);
        }
    }
}