using Generator.Core;
using System.Linq;
using System.Text;
using TypeSystem.Data.ApiCatalog;

namespace Generator.Venn
{
    public class VennGenerator : BaseGenerator<string, CatalogObject>
    {
        public override string GenerateSpecificDataObject(CatalogObject dataObject)
        {
            return GenerateVenn(dataObject, null);
        }

        public void WriteAllSpecifications(CatalogObject dataObject, string path, string templateFile)
        {
            path = FixPath(path);
            foreach (var specShortName in dataObject.Specifications.Keys)
            {
                var specVenn = GenerateVenn(dataObject, specShortName);
                Write($"{path}data\\{specShortName}.json", specVenn);

                var vennPage = GenerateVenPageFromTemplate(specShortName, templateFile);
                Write($"{path}pages\\{specShortName}.htm", vennPage);
            }
        }

        private static string GenerateVenPageFromTemplate(string specShortName, string templateFile)
        {

            return "";
        }

        private static string FixPath(string path)
        {
            return path[path.Length-1] == '\\' ? path : path + "\\";
        }

        private static string GenerateVenn(CatalogObject dataObject, string specName)
        {
            var filteredBySpecInterfaceObjects = dataObject.Interfaces.Values.Where(a => FilterInterfaceSpec(a, specName));
            var filteredBySpecMemberObjects = filteredBySpecInterfaceObjects.SelectMany(a => a.Members.Values.Where(b => FilterMemberSpec(b, specName))).ToList();

            var browsers = dataObject.Browsers;

            var sb = new StringBuilder();

            sb.Append("[");

            var comma = string.Empty;
            for (var i = 0; i < browsers.Count; i++)
            {
                sb.AppendLine(comma).Append($"{{ \"sets\": [{i}], \"label\": \"{browsers[i]}\", \"size\": {filteredBySpecMemberObjects.Count(a => a.SupportedBrowsers.Contains(browsers[i]))}, \"lookup\": \"{browsers[i]}\" }}");
                comma = ",";
            }

            var startJ = 1;
            for (var i = 0; i < browsers.Count; i++)
            {
                for (var j = startJ; j < browsers.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    sb.AppendLine(comma).Append($"{{ \"sets\": [{i},{j}], \"size\": {filteredBySpecMemberObjects.Count(a => a.SupportedBrowsers.Contains(browsers[i]) && a.SupportedBrowsers.Contains(browsers[j]))} }}");
                }
                startJ++;
            }

            startJ = 1;
            for (var i = 0; i < browsers.Count; i++)
            {
                var startK = 2;
                for (var j = startJ; j < browsers.Count - 1; j++)
                {
                    if (j <= i)
                    {
                        continue;
                    }
                    for (var k = startK; k < browsers.Count; k++)
                    {
                        if (k <= i || k <= j)
                        {
                            continue;
                        }
                        sb.AppendLine(comma).Append($"{{ \"sets\": [{i},{j},{k}], \"size\": {filteredBySpecMemberObjects.Count(a => a.SupportedBrowsers.Contains(browsers[i]) && a.SupportedBrowsers.Contains(browsers[j]) && a.SupportedBrowsers.Contains(browsers[k]))} }}");
                    }
                    startK++;
                }
                startJ++;
            }

            sb.AppendLine().Append("]");

            return sb.ToString();
        }

        private static bool FilterInterfaceSpec(InterfaceObject interfaceData, string specName)
        {
            if (string.IsNullOrWhiteSpace(specName))
            {
                return true;
            }
            return interfaceData.SpecificationNames != null && interfaceData.SpecificationNames.Contains(specName);
        }

        private static bool FilterMemberSpec(MemberObject memberData, string specName)
        {
            if (string.IsNullOrWhiteSpace(specName))
            {
                return true;
            }
            return memberData.SpecificationNames != null && memberData.SpecificationNames.Contains(specName);
        }
    }
}